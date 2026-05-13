using Common.Configurations;
using Common.Markers;
using Entities.Accounts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services.Contracts.Authentications;
using Services.Contracts.Notifiers;
using Services.Contracts.Repositories;
using Services.DTOs.Accounts.CompleteRegistration;
using Services.DTOs.Accounts.Authorization;
using Services.DTOs.Accounts.Login;
using Services.DTOs.Accounts.Mfa;
using Services.DTOs.Accounts.Otp;
using Services.DTOs.Accounts.Recovery;
using Services.DTOs.Accounts.Registration;
using Services.DTOs.Accounts.SendOtp;
using Services.DTOs.Accounts.Sessions;
using Services.DTOs.Accounts.StartAuthentication;
using Services.DTOs.Accounts.ValidateOtp;
using Services.DTOs.Mails;
using Services.DTOs.Users.UpdatePassword;
using System.Security.Cryptography;
using System.Text;

namespace Services.Services.Authentications;

public class AuthenticationService : IAuthenticationService, IScopedDependency
{
    private const int OtpExpireSeconds = 120;
    private const int ResendActivationCooldownSeconds = 60;
    private const string MfaFeatureCode = "MFA_REQUIRED";

    private readonly IJwtService _jwtService;
    private readonly IEffectiveAuthorizationService _effectiveAuthorizationService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserContext _userContext;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ISenderService _senderService;
    private readonly IRepository<AccountUserSession> _sessionRepository;
    private readonly IRepository<AccountSecurityFeature> _securityFeatureRepository;
    private readonly IRepository<AccountUserSecuritySetting> _userSecuritySettingRepository;
    private readonly IRepository<ConfirmationCode> _confirmationCodeRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SiteSettings _siteSettings;

    public AuthenticationService(
        IJwtService jwtService,
        IEffectiveAuthorizationService effectiveAuthorizationService,
        ISenderService senderService,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserContext userContext,
        IWebHostEnvironment hostingEnvironment,
        IRepository<AccountUserSession> sessionRepository,
        IRepository<AccountSecurityFeature> securityFeatureRepository,
        IRepository<AccountUserSecuritySetting> userSecuritySettingRepository,
        IRepository<ConfirmationCode> confirmationCodeRepository,
        IHttpContextAccessor httpContextAccessor,
        IOptionsSnapshot<SiteSettings> siteSettings)
    {
        _jwtService = jwtService;
        _effectiveAuthorizationService = effectiveAuthorizationService;
        _senderService = senderService;
        _userManager = userManager;
        _signInManager = signInManager;
        _userContext = userContext;
        _hostingEnvironment = hostingEnvironment;
        _sessionRepository = sessionRepository;
        _securityFeatureRepository = securityFeatureRepository;
        _userSecuritySettingRepository = userSecuritySettingRepository;
        _confirmationCodeRepository = confirmationCodeRepository;
        _httpContextAccessor = httpContextAccessor;
        _siteSettings = siteSettings.Value;
    }

    public async Task<OtpChallengeResponse> RegisterWithEmailAsync(RegisterWithEmailRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var duplicateEmail = await _userManager.Users.AnyAsync(
            u => u.Email != null && u.Email.ToUpper() == email.ToUpper(),
            cancellationToken);

        if (duplicateEmail)
            throw new AppException(ApplicationMessages.DuplicateEmail);

        var otp = GenerateOtp();

        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = request.FullName?.Trim(),
            IsActive = false,
            EmailConfirmed = false,
            CreatedDate = DateTime.UtcNow,
            ConfirmationCode = new ConfirmationCode
            {
                NewEmailOtp = otp,
                NewEmailOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds)
            }
        };

        await CreateUserAsync(user, request.Password);
        await AddDefaultRoleToUserAsync(user);
        await SendActivationEmailAsync(user, email, otp, cancellationToken);

        return new OtpChallengeResponse(user.Id, email, OtpExpireSeconds);
    }

    public async Task ConfirmEmailRegistrationAsync(ConfirmEmailRegistrationRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId && u.Email != null && u.Email.ToUpper() == email.ToUpper(),
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (user.EmailConfirmed)
        {
            if (!user.IsActive)
            {
                user.IsActive = true;
                await UpdateUserAsync(user);
            }

            return;
        }

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);

        if (!confirmation.NewEmailOtpExpirationDate.HasValue || confirmation.NewEmailOtpExpirationDate.Value < DateTime.UtcNow)
            throw new BadRequestException(ApplicationMessages.ExpiredOtp);

        if (confirmation.NewEmailOtp != request.Otp.Trim())
            throw new BadRequestException(ApplicationMessages.InvalidOtp);

        user.IsActive = true;
        user.EmailConfirmed = true;
        confirmation.NewEmailOtp = null;
        confirmation.NewEmailOtpExpirationDate = null;

        await UpdateUserAsync(user);
    }

    public async Task<OtpChallengeResponse> ResendEmailActivationLinkAsync(ResendEmailActivationLinkRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId && u.Email != null && u.Email.ToUpper() == email.ToUpper(),
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (user.EmailConfirmed)
            throw new AppException(ApplicationMessages.EmailAlreadyConfirmed);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        if (HasRecentActivationEmail(confirmation))
            throw new AppException(ApplicationMessages.ResendActivationTooSoon);

        var otp = GenerateOtp();
        confirmation.NewEmailOtp = otp;
        confirmation.NewEmailOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds);
        await UpdateUserAsync(user);

        await SendActivationEmailAsync(user, email, otp, cancellationToken);

        return new OtpChallengeResponse(user.Id, email, OtpExpireSeconds);
    }

    public async Task<OtpChallengeResponse> StartPhoneRegistrationAsync(StartPhoneRegistrationRequest request, CancellationToken cancellationToken)
    {
        var mobile = request.Mobile.Trim();
        var duplicateMobile = await _userManager.Users.AnyAsync(
            u => u.Mobile != null && u.Mobile == mobile,
            cancellationToken);

        if (duplicateMobile)
            throw new AppException(ApplicationMessages.DuplicatePhoneNumber);

        var otp = GenerateOtp();

        var user = new User
        {
            UserName = mobile,
            Mobile = mobile,
            FullName = request.FullName?.Trim(),
            IsActive = false,
            PhoneNumberConfirmed = false,
            CreatedDate = DateTime.UtcNow,
            ConfirmationCode = new ConfirmationCode
            {
                NewPhoneNumberOtp = otp,
                NewPhoneNumberOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds)
            }
        };

        await CreateUserAsync(user);
        await AddDefaultRoleToUserAsync(user);

        var otpSendResult = _hostingEnvironment.IsDevelopment() || await _senderService.SendOtpSmsAsync(mobile, otp, cancellationToken);
        if (!otpSendResult)
            throw new AppException(ApplicationMessages.ErrorInSendOtp);

        return new OtpChallengeResponse(user.Id, mobile, OtpExpireSeconds);
    }

    public async Task<OtpChallengeResponse> ResendPhoneRegistrationOtpAsync(ResendPhoneRegistrationOtpRequest request, CancellationToken cancellationToken)
    {
        var mobile = request.Mobile.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId && u.Mobile != null && u.Mobile == mobile,
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (user.PhoneNumberConfirmed)
            throw new AppException(ApplicationMessages.PhoneNumberAlreadyConfirmed);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        if (HasRecentPhoneRegistrationOtp(confirmation))
            throw new AppException(ApplicationMessages.ResendActivationTooSoon);

        var otp = GenerateOtp();
        confirmation.NewPhoneNumberOtp = otp;
        confirmation.NewPhoneNumberOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds);
        await UpdateUserAsync(user);

        var otpSendResult = _hostingEnvironment.IsDevelopment() || await _senderService.SendOtpSmsAsync(mobile, otp, cancellationToken);
        if (!otpSendResult)
            throw new AppException(ApplicationMessages.ErrorInSendOtp);

        return new OtpChallengeResponse(user.Id, mobile, OtpExpireSeconds);
    }

    public async Task CompletePhoneRegistrationAsync(CompletePhoneRegistrationRequest request, CancellationToken cancellationToken)
    {
        var mobile = request.Mobile.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId && u.Mobile != null && u.Mobile == mobile,
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);

        if (!confirmation.NewPhoneNumberOtpExpirationDate.HasValue || confirmation.NewPhoneNumberOtpExpirationDate.Value < DateTime.UtcNow)
            throw new AppException(ApplicationMessages.ExpiredOtp);

        if (confirmation.NewPhoneNumberOtp != request.Otp.Trim())
            throw new AppException(ApplicationMessages.InvalidOtp);

        user.IsActive = true;
        user.PhoneNumberConfirmed = true;
        confirmation.NewPhoneNumberOtp = null;
        confirmation.NewPhoneNumberOtpExpirationDate = null;

        await UpdateUserAsync(user);
    }

    public async Task<LoginResponse> EmailPasswordLoginAsync(EmailPasswordLoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Email != null && u.Email.ToUpper() == email.ToUpper(),
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (!user.IsActive)
            throw new AppException(ApplicationMessages.UserIsDeActive);

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!checkPasswordResult)
            throw new AppException(ApplicationMessages.InvalidUserNameOrPassword);

        await _signInManager.SignInAsync(user, false);
        var tokenData = await _jwtService.GenerateAsync(user);
        await CreateOrUpdateSessionAsync(user, tokenData.AccessToken, "email_password", cancellationToken);
        return BuildLoginResponse(user, tokenData.AccessToken);
    }

    public async Task<OtpChallengeResponse> SendPhoneLoginOtpAsync(SendPhoneLoginOtpRequest request, CancellationToken cancellationToken)
    {
        var mobile = request.Mobile.Trim();

        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Mobile != null && u.Mobile == mobile,
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (!user.IsActive)
            throw new AppException(ApplicationMessages.UserIsDeActive);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        var otp = GenerateOtp();

        confirmation.LoginOtp = otp;
        confirmation.LoginOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds);
        await UpdateUserAsync(user);

        var otpSendResult =  await _senderService.SendOtpSmsAsync(mobile, otp, cancellationToken);
        if (!otpSendResult)
            throw new AppException(ApplicationMessages.ErrorInSendOtp);

        return new OtpChallengeResponse(user.Id, mobile, OtpExpireSeconds);
    }

    public async Task<LoginResponse> PhoneOtpLoginAsync(PhoneOtpLoginRequest request, CancellationToken cancellationToken)
    {
        var mobile = request.Mobile.Trim();
        var otp = request.Otp.Trim();

        var user = await _userManager.Users.FirstOrDefaultAsync(
            u => u.Mobile != null && u.Mobile == mobile,
            cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (!user.IsActive)
            throw new AppException(ApplicationMessages.UserIsDeActive);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        if (!confirmation.LoginOtpExpirationDate.HasValue || confirmation.LoginOtpExpirationDate.Value < DateTime.UtcNow)
            throw new AppException(ApplicationMessages.ExpiredOtp);

        if (confirmation.LoginOtp != otp)
            throw new AppException(ApplicationMessages.InvalidOtp);

        await _signInManager.SignInAsync(user, false);
        var tokenData = await _jwtService.GenerateAsync(user);
        await CreateOrUpdateSessionAsync(user, tokenData.AccessToken, "phone_otp", cancellationToken);
        return BuildLoginResponse(user, tokenData.AccessToken);
    }

    public async Task<LoginResponse> PasswordLoginAsync(PasswordLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!user.IsActive)
            throw new AppException(ApplicationMessages.UserIsDeActive);

        if (!checkPasswordResult)
            throw new AppException(ApplicationMessages.InvalidPassword);

        await _signInManager.SignInAsync(user, false);
        var tokenData = await _jwtService.GenerateAsync(user);
        await CreateOrUpdateSessionAsync(user, tokenData.AccessToken, "password", cancellationToken);
        return BuildLoginResponse(user, tokenData.AccessToken);
    }

    public async Task<LoginResponse> OtpLoginAsync(ValidateOtpRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (!user.IsActive)
            throw new AppException(ApplicationMessages.UserIsDeActive);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);

        if (!confirmation.LoginOtpExpirationDate.HasValue || confirmation.LoginOtpExpirationDate.Value < DateTime.UtcNow)
            throw new AppException(ApplicationMessages.ExpiredOtp);

        if (confirmation.LoginOtp != request.Otp)
            throw new AppException(ApplicationMessages.InvalidOtp);

        await _signInManager.SignInAsync(user, false);
        var tokenData = await _jwtService.GenerateAsync(user);
        await CreateOrUpdateSessionAsync(user, tokenData.AccessToken, "otp", cancellationToken);
        return BuildLoginResponse(user, tokenData.AccessToken);
    }

    public async Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (string.IsNullOrWhiteSpace(user.Mobile))
            throw new AppException(ApplicationMessages.UserHasNoValidPhoneNumber);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        var otp = GenerateOtp();

        confirmation.LoginOtp = otp;
        confirmation.LoginOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds);
        await UpdateUserAsync(user);

        var otpSendResult = _hostingEnvironment.IsDevelopment() || await _senderService.SendOtpSmsAsync(user.Mobile, otp, cancellationToken);
        if (!otpSendResult)
            throw new AppException(ApplicationMessages.ErrorInSendOtp);

        return new SendOtpResponse(request.AccountId, user.Id);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        // Security note: this flow always returns success to avoid user enumeration.
        var user = await FindUserByIdentifierAsync(request.Email, request.Mobile, cancellationToken);
        if (user == null || !user.IsActive)
            return;

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        var otp = GenerateOtp();
        confirmation.UpdatePasswordOtp = otp;
        confirmation.UpdatePasswordOtpExpirationDate = DateTime.UtcNow.AddSeconds(OtpExpireSeconds);
        await UpdateUserAsync(user);

        await SendRecoveryOtpAsync(user, otp, cancellationToken);
    }

    public async Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpRequest request, CancellationToken cancellationToken)
    {
        var user = await FindUserByIdentifierAsync(request.Email, request.Mobile, cancellationToken)
            ?? throw new BadRequestException(ApplicationMessages.InvalidOtp);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        if (!confirmation.UpdatePasswordOtpExpirationDate.HasValue || confirmation.UpdatePasswordOtpExpirationDate.Value < DateTime.UtcNow)
            throw new BadRequestException(ApplicationMessages.ExpiredOtp);

        if (confirmation.UpdatePasswordOtp != request.Otp.Trim())
            throw new BadRequestException(ApplicationMessages.InvalidOtp);

        // Reset flow: remove previous password hash and set a new one after OTP validation.
        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (hasPassword)
        {
            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                throw new BadRequestException(ApplicationMessages.ServerError);
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!addPasswordResult.Succeeded)
            throw new BadRequestException(ApplicationMessages.TryToCreateInvalidPassword);

        confirmation.UpdatePasswordOtp = null;
        confirmation.UpdatePasswordOtpExpirationDate = null;
        await UpdateUserAsync(user);

        await RevokeAllUserSessionsAsync(user.Id, "password_reset", cancellationToken);
    }

    public async Task<IReadOnlyList<AccountSessionItemDto>> GetMySessionsAsync(CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.TableNoTracking
            .TagWith("Auth.MySessions")
            .Where(x => x.UserId == MapUserIdToLegacyInt(_userContext.UserId) && x.RevokedAt == null)
            .OrderByDescending(x => x.LastSeenAt)
            .ToListAsync(cancellationToken);

        var currentSessionHash = GetCurrentBearerHash();
        return sessions.Select(x => new AccountSessionItemDto(
            x.SessionPublicId,
            x.AuthMethod,
            x.DeviceType,
            x.DeviceName,
            x.OsName,
            x.BrowserName,
            x.IpAddress?.ToString(),
            x.IssuedAt,
            x.LastSeenAt,
            x.ExpiresAt,
            !string.IsNullOrWhiteSpace(currentSessionHash) && x.SessionSecretHash == currentSessionHash)).ToList();
    }

    public async Task RevokeSessionAsync(RevokeSessionRequest request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.Table
            .FirstOrDefaultAsync(x => x.SessionPublicId == request.SessionPublicId && x.UserId == MapUserIdToLegacyInt(_userContext.UserId), cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.SessionNotFound);

        if (session.RevokedAt == null)
        {
            session.RevokedAt = DateTimeOffset.UtcNow;
            session.RevokeReason = "user_revoked";
            session.UpdatedAt = DateTimeOffset.UtcNow;
            await _sessionRepository.UpdateAsync(session, cancellationToken);
        }
    }

    public async Task RevokeOtherSessionsAsync(CancellationToken cancellationToken)
    {
        var currentSessionHash = GetCurrentBearerHash();
        var sessions = await _sessionRepository.Table
            .Where(x => x.UserId == MapUserIdToLegacyInt(_userContext.UserId) && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions.Where(x => x.SessionSecretHash != currentSessionHash))
        {
            session.RevokedAt = DateTimeOffset.UtcNow;
            session.RevokeReason = "revoke_other_sessions";
            session.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _sessionRepository.UpdateRangeAsync(sessions, cancellationToken);
    }

    public async Task<MfaStatusResponse> GetMfaStatusAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString())
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var roles = await _userManager.GetRolesAsync(user);
        var isRequiredByRole = roles.Any(r => r == RoleHelper.Admin || r == RoleHelper.SuperAdmin);
        var feature = await EnsureMfaFeatureAsync(cancellationToken);
        var userSetting = await _userSecuritySettingRepository.TableNoTracking
            .FirstOrDefaultAsync(x => x.UserId == MapUserIdToLegacyInt(_userContext.UserId) && x.FeatureId == feature.Id, cancellationToken);

        var isEnabled = userSetting?.IsEnabled ?? feature.DefaultEnabled;
        return new MfaStatusResponse(isEnabled, isRequiredByRole);
    }

    public async Task SetMfaStatusAsync(SetMfaStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString())
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        // Dependency note: changing MFA state requires OTP challenge to prevent account takeover.
        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);
        if (!confirmation.LoginOtpExpirationDate.HasValue || confirmation.LoginOtpExpirationDate.Value < DateTime.UtcNow || confirmation.LoginOtp != request.Otp.Trim())
            throw new BadRequestException(ApplicationMessages.MfaRequiresOtp);

        var feature = await EnsureMfaFeatureAsync(cancellationToken);
        var setting = await _userSecuritySettingRepository.Table
            .FirstOrDefaultAsync(x => x.UserId == MapUserIdToLegacyInt(_userContext.UserId) && x.FeatureId == feature.Id, cancellationToken);

        if (setting == null)
        {
            setting = new AccountUserSecuritySetting
            {
                UserId = MapUserIdToLegacyInt(_userContext.UserId),
                FeatureId = feature.Id,
                IsEnabled = request.IsEnabled,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            await _userSecuritySettingRepository.AddAsync(setting, cancellationToken);
        }
        else
        {
            setting.IsEnabled = request.IsEnabled;
            setting.UpdatedAt = DateTimeOffset.UtcNow;
            await _userSecuritySettingRepository.UpdateAsync(setting, cancellationToken);
        }
    }

    public async Task<CurrentUserAuthorizationResponse> GetMyAuthorizationAsync(CancellationToken cancellationToken)
        => await _effectiveAuthorizationService.GetEffectiveAuthorizationAsync(_userContext.UserId, cancellationToken);

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _userContext.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var currentSessionHash = GetCurrentBearerHash();
        if (!string.IsNullOrWhiteSpace(currentSessionHash))
        {
            var currentSession = await _sessionRepository.Table
                .TagWith("Auth.LogoutCurrentSession")
                .FirstOrDefaultAsync(
                    x => x.UserId == MapUserIdToLegacyInt(user.Id)
                        && x.SessionSecretHash == currentSessionHash
                        && x.RevokedAt == null,
                    cancellationToken);

            if (currentSession != null)
            {
                currentSession.RevokedAt = DateTimeOffset.UtcNow;
                currentSession.RevokeReason = "user_logout";
                currentSession.UpdatedAt = DateTimeOffset.UtcNow;
                await _sessionRepository.UpdateAsync(currentSession, cancellationToken);
            }
        }

        await _signInManager.SignOutAsync();
    }

    public async Task UpdatePasswordAsync(UpdatePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString())
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var confirmation = await GetOrCreateConfirmationCodeAsync(user, cancellationToken);

        if (!confirmation.UpdatePasswordOtpExpirationDate.HasValue || confirmation.UpdatePasswordOtpExpirationDate.Value < DateTime.UtcNow)
            throw new BadRequestException(ApplicationMessages.ExpiredOtp);

        if (confirmation.UpdatePasswordOtp != request.Otp)
            throw new BadRequestException(ApplicationMessages.InvalidOtp);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            throw new BadRequestException(ApplicationMessages.InvalidPassword);
    }

    #region Private Methods
    private async Task AddDefaultRoleToUserAsync(User user)
    {
        var addRoleResult = await _userManager.AddToRoleAsync(user, RoleHelper.User);
        if (!addRoleResult.Succeeded)
            throw new AppException(ApplicationMessages.ErrorInUpdateUserPermissions);
    }

    private async Task UpdateUserAsync(User user)
    {
        var userUpdateResult = await _userManager.UpdateAsync(user);
        if (!userUpdateResult.Succeeded)
            throw new AppException(ApplicationMessages.ServerError);
    }

    private async Task UpdateUserAsync(User user, string password)
    {
        var updateResult = await _userManager.AddPasswordAsync(user, password);
        if (!updateResult.Succeeded)
            throw new AppException(ApplicationMessages.ErrorInUpdateUser);
    }

    private async Task CreateUserAsync(User user)
    {
        var userCreationResult = await _userManager.CreateAsync(user);
        if (!userCreationResult.Succeeded)
            throw new AppException(ApplicationMessages.ErrorInCreateUser);
    }

    private async Task CreateUserAsync(User user, string password)
    {
        var userCreationResult = await _userManager.CreateAsync(user, password);
        if (!userCreationResult.Succeeded)
            throw new AppException(ApplicationMessages.ErrorInCreateUser);
    }

    private string GenerateOtp() => _hostingEnvironment.IsDevelopment() ? "123456" : CodeGenerator.GenerateRandomNumber(6);

    private async Task<ConfirmationCode> GetOrCreateConfirmationCodeAsync(User user, CancellationToken cancellationToken)
    {
        if (user.ConfirmationCodeId > 0)
        {
            var existing = await _confirmationCodeRepository.Entities
                .FirstOrDefaultAsync(x => x.Id == user.ConfirmationCodeId, cancellationToken);
            if (existing != null)
            {
                user.ConfirmationCode = existing;
                return existing;
            }
        }

        var confirmation = new ConfirmationCode();
        await _confirmationCodeRepository.AddAsync(confirmation, cancellationToken);

        user.ConfirmationCodeId = confirmation.Id;
        user.ConfirmationCode = confirmation;
        await UpdateUserAsync(user);

        return confirmation;
    }

    private static LoginResponse BuildLoginResponse(User user, string token)
    {
        return new LoginResponse(
            Token: token,
            Email: user.Email,
            Mobile: user.Mobile,
            FullName: user.FullName);
    }

    private string BuildEmailActivationLink(User user, string email, string otp)
    {
        var baseConfirmUrl = _siteSettings.MailSettings?.ActivationConfirmUrl?.Trim();
        if (string.IsNullOrWhiteSpace(baseConfirmUrl))
            return string.Empty;

        var separator = baseConfirmUrl.Contains('?') ? "&" : "?";
        var encodedEmail = Uri.EscapeDataString(email);
        return $"{baseConfirmUrl}{separator}userId={user.Id}&email={encodedEmail}&otp={otp}";
    }

    private async Task SendActivationEmailAsync(User user, string email, string otp, CancellationToken cancellationToken)
    {
        var mailSettings = _siteSettings.MailSettings ?? new MailSettings();
        var activationLink = BuildEmailActivationLink(user, email, otp);

        var emailSendResult = await _senderService.SendEmailAsync(new PostalServerMailRequest
        {
            To = [email],
            From = mailSettings.Mail,
            ReplyTo = mailSettings.Mail,
            Subject = "Account Activation",
            PlainBody = $"Your verification code is: {otp}{Environment.NewLine}Activation link: {activationLink}",
            HtmlBody = $"<p>Your verification code is: <strong>{otp}</strong></p><p>Activation link: <a href=\"{activationLink}\">{activationLink}</a></p>",
            Attachments = []
        }, cancellationToken);

        if (!emailSendResult)
            throw new AppException(ApplicationMessages.ErrorInSendOtp);
    }

    private static bool HasRecentActivationEmail(ConfirmationCode confirmation)
    {
        if (!confirmation.NewEmailOtpExpirationDate.HasValue)
            return false;

        return confirmation.NewEmailOtpExpirationDate.Value > DateTime.UtcNow.AddSeconds(OtpExpireSeconds - ResendActivationCooldownSeconds);
    }

    private static bool HasRecentPhoneRegistrationOtp(ConfirmationCode confirmation)
    {
        if (!confirmation.NewPhoneNumberOtpExpirationDate.HasValue)
            return false;

        return confirmation.NewPhoneNumberOtpExpirationDate.Value > DateTime.UtcNow.AddSeconds(OtpExpireSeconds - ResendActivationCooldownSeconds);
    }

    private async Task<User?> FindUserByIdentifierAsync(string? email, string? mobile, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.Trim().ToUpper();
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToUpper() == normalizedEmail, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(mobile))
        {
            var normalizedMobile = mobile.Trim();
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Mobile == normalizedMobile, cancellationToken);
        }

        return null;
    }

    private async Task SendRecoveryOtpAsync(User user, string otp, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(user.Mobile))
        {
            var smsResult = _hostingEnvironment.IsDevelopment() || await _senderService.SendOtpSmsAsync(user.Mobile, otp, cancellationToken);
            if (smsResult)
                return;
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
            await SendActivationEmailAsync(user, user.Email, otp, cancellationToken);
    }

    private async Task CreateOrUpdateSessionAsync(User user, string accessToken, string authMethod, CancellationToken cancellationToken)
    {
        // Flow note: we keep a hashed token fingerprint (never raw token) for revoke/list operations.
        var utcNow = DateTimeOffset.UtcNow;
        var userAgent = _httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.ToString();
        var tokenHash = ComputeSha256(accessToken);

        var session = new AccountUserSession
        {
            SessionPublicId = Guid.NewGuid(),
            UserId = MapUserIdToLegacyInt(user.Id),
            SessionSecretHash = tokenHash,
            AuthMethod = authMethod,
            DeviceType = ParseDeviceType(userAgent),
            DeviceName = _httpContextAccessor.HttpContext?.Request?.Headers["sec-ch-ua-platform"].ToString(),
            OsName = _httpContextAccessor.HttpContext?.Request?.Headers["sec-ch-ua-platform"].ToString(),
            BrowserName = _httpContextAccessor.HttpContext?.Request?.Headers["sec-ch-ua"].ToString(),
            IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress,
            UserAgent = userAgent,
            IssuedAt = utcNow,
            LastSeenAt = utcNow,
            ExpiresAt = utcNow.AddDays(7),
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        await _sessionRepository.AddAsync(session, cancellationToken);
    }

    private async Task RevokeAllUserSessionsAsync(Guid userId, string reason, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var sessions = await _sessionRepository.Table
            .Where(x => x.UserId == MapUserIdToLegacyInt(userId) && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.RevokedAt = now;
            session.RevokeReason = reason;
            session.UpdatedAt = now;
        }

        if (sessions.Count > 0)
            await _sessionRepository.UpdateRangeAsync(sessions, cancellationToken);
    }

    private async Task<AccountSecurityFeature> EnsureMfaFeatureAsync(CancellationToken cancellationToken)
    {
        var feature = await _securityFeatureRepository.Table.FirstOrDefaultAsync(x => x.Code == MfaFeatureCode, cancellationToken);
        if (feature != null)
            return feature;

        feature = new AccountSecurityFeature
        {
            Code = MfaFeatureCode,
            Name = "Multi Factor Authentication",
            Description = "Per-user MFA toggle",
            DefaultEnabled = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _securityFeatureRepository.AddAsync(feature, cancellationToken);
        return feature;
    }

    private string? GetCurrentBearerHash()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request?.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        var token = authHeader["Bearer ".Length..].Trim();
        return string.IsNullOrWhiteSpace(token) ? null : ComputeSha256(token);
    }

    private static int MapUserIdToLegacyInt(Guid userId)
    {
        // Compatibility bridge for legacy tables that still use int user_id.
        return Math.Abs(BitConverter.ToInt32(userId.ToByteArray(), 0));
    }

    private static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ParseDeviceType(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return "unknown";

        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            return "mobile";

        return "web";
    }

    #endregion
}


