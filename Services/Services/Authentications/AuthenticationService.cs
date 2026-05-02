using Common.Configurations;
using Common.Markers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services.Contracts.Authentications;
using Services.Contracts.Notifiers;
using Services.DTOs.Accounts.CompleteRegistration;
using Services.DTOs.Accounts.Login;
using Services.DTOs.Accounts.Otp;
using Services.DTOs.Accounts.Registration;
using Services.DTOs.Accounts.SendOtp;
using Services.DTOs.Accounts.StartAuthentication;
using Services.DTOs.Accounts.ValidateOtp;
using Services.DTOs.Mails;
using Services.DTOs.Users.UpdatePassword;

namespace Services.Services.Authentications;

public class AuthenticationService : IAuthenticationService, IScopedDependency
{
    private const int OtpExpireSeconds = 120;
    private const int ResendActivationCooldownSeconds = 60;

    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserContext _userContext;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ISenderService _senderService;
    private readonly SiteSettings _siteSettings;

    public AuthenticationService(
        IJwtService jwtService,
        ISenderService senderService,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserContext userContext,
        IWebHostEnvironment hostingEnvironment,
        IOptionsSnapshot<SiteSettings> siteSettings)
    {
        _jwtService = jwtService;
        _senderService = senderService;
        _userManager = userManager;
        _signInManager = signInManager;
        _userContext = userContext;
        _hostingEnvironment = hostingEnvironment;
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
            CreatedDate = DateTime.Now,
            ConfirmationCode = new ConfirmationCode
            {
                NewEmailOtp = otp,
                NewEmailOtpExpirationDate = DateTime.Now.AddSeconds(OtpExpireSeconds)
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

        var confirmation = EnsureConfirmationCode(user);

        if (!confirmation.NewEmailOtpExpirationDate.HasValue || confirmation.NewEmailOtpExpirationDate.Value < DateTime.Now)
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

        var confirmation = EnsureConfirmationCode(user);
        if (HasRecentActivationEmail(confirmation))
            throw new AppException(ApplicationMessages.ResendActivationTooSoon);

        var otp = GenerateOtp();
        confirmation.NewEmailOtp = otp;
        confirmation.NewEmailOtpExpirationDate = DateTime.Now.AddSeconds(OtpExpireSeconds);
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
            CreatedDate = DateTime.Now,
            ConfirmationCode = new ConfirmationCode
            {
                NewPhoneNumberOtp = otp,
                NewPhoneNumberOtpExpirationDate = DateTime.Now.AddSeconds(OtpExpireSeconds)
            }
        };

        await CreateUserAsync(user);
        await AddDefaultRoleToUserAsync(user);

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

        var confirmation = EnsureConfirmationCode(user);

        if (!confirmation.NewPhoneNumberOtpExpirationDate.HasValue || confirmation.NewPhoneNumberOtpExpirationDate.Value < DateTime.Now)
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

        var confirmation = EnsureConfirmationCode(user);
        var otp = GenerateOtp();

        confirmation.LoginOtp = otp;
        confirmation.LoginOtpExpirationDate = DateTime.Now.AddSeconds(OtpExpireSeconds);
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

        var confirmation = EnsureConfirmationCode(user);
        if (!confirmation.LoginOtpExpirationDate.HasValue || confirmation.LoginOtpExpirationDate.Value < DateTime.Now)
            throw new AppException(ApplicationMessages.ExpiredOtp);

        if (confirmation.LoginOtp != otp)
            throw new AppException(ApplicationMessages.InvalidOtp);

        await _signInManager.SignInAsync(user, false);
        var tokenData = await _jwtService.GenerateAsync(user);
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
        return BuildLoginResponse(user, tokenData.AccessToken);
    }

    public async Task<LoginResponse> OtpLoginAsync(ValidateOtpRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (!user.IsActive)
            throw new AppException(ApplicationMessages.UserIsDeActive);

        var confirmation = EnsureConfirmationCode(user);

        if (!confirmation.LoginOtpExpirationDate.HasValue || confirmation.LoginOtpExpirationDate.Value < DateTime.Now)
            throw new AppException(ApplicationMessages.ExpiredOtp);

        if (confirmation.LoginOtp != request.Otp)
            throw new AppException(ApplicationMessages.InvalidOtp);

        await _signInManager.SignInAsync(user, false);
        var tokenData = await _jwtService.GenerateAsync(user);
        return BuildLoginResponse(user, tokenData.AccessToken);
    }

    public async Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        if (string.IsNullOrWhiteSpace(user.Mobile))
            throw new AppException(ApplicationMessages.UserHasNoValidPhoneNumber);

        var confirmation = EnsureConfirmationCode(user);
        var otp = GenerateOtp();

        confirmation.LoginOtp = otp;
        confirmation.LoginOtpExpirationDate = DateTime.Now.AddSeconds(OtpExpireSeconds);
        await UpdateUserAsync(user);

        var otpSendResult = _hostingEnvironment.IsDevelopment() || await _senderService.SendOtpSmsAsync(user.Mobile, otp, cancellationToken);
        if (!otpSendResult)
            throw new AppException(ApplicationMessages.ErrorInSendOtp);

        return new SendOtpResponse(request.AccountId, user.Id);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _userContext.UserId, cancellationToken)
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        await _signInManager.SignOutAsync();
    }

    public async Task UpdatePasswordAsync(UpdatePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(_userContext.UserId.ToString())
            ?? throw new NotFoundException(ApplicationMessages.UserNotFound);

        var confirmation = EnsureConfirmationCode(user);

        if (!confirmation.UpdatePasswordOtpExpirationDate.HasValue || confirmation.UpdatePasswordOtpExpirationDate.Value < DateTime.Now)
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

    private static ConfirmationCode EnsureConfirmationCode(User user)
    {
        user.ConfirmationCode ??= new ConfirmationCode();
        return user.ConfirmationCode;
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

        return confirmation.NewEmailOtpExpirationDate.Value > DateTime.Now.AddSeconds(OtpExpireSeconds - ResendActivationCooldownSeconds);
    }

    #endregion
}


