using Entities.Users;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.Authentications;
using Services.DTOs.Accounts.Login;
using Services.DTOs.Accounts.Mfa;
using Services.DTOs.Accounts.Otp;
using Services.DTOs.Accounts.Authorization;
using Services.DTOs.Accounts.Recovery;
using Services.DTOs.Accounts.Registration;
using Services.DTOs.Accounts.SendOtp;
using Services.DTOs.Accounts.Sessions;
using Services.DTOs.Accounts.ValidateOtp;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.Auth.v1
{
    [ApiVersion("1")]
    public class AuthController(IAuthenticationService authService) : BaseUserApiController
    {
        [HttpPost("[action]")]
        public async Task<OtpChallengeResponse> RegisterWithEmailAsync(RegisterWithEmailRequest request, CancellationToken cancellationToken)
            => await authService.RegisterWithEmailAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task ConfirmEmailRegistrationAsync(ConfirmEmailRegistrationRequest request, CancellationToken cancellationToken)
            => await authService.ConfirmEmailRegistrationAsync(request, cancellationToken);

        [HttpGet("[action]")]
        public async Task ConfirmEmailRegistrationByLinkAsync([FromQuery] ConfirmEmailRegistrationRequest request, CancellationToken cancellationToken)
            => await authService.ConfirmEmailRegistrationAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<OtpChallengeResponse> ResendEmailActivationLinkAsync(ResendEmailActivationLinkRequest request, CancellationToken cancellationToken)
            => await authService.ResendEmailActivationLinkAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<OtpChallengeResponse> StartPhoneRegistrationAsync(StartPhoneRegistrationRequest request, CancellationToken cancellationToken)
            => await authService.StartPhoneRegistrationAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<OtpChallengeResponse> ResendPhoneRegistrationOtpAsync(ResendPhoneRegistrationOtpRequest request, CancellationToken cancellationToken)
            => await authService.ResendPhoneRegistrationOtpAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task CompletePhoneRegistrationAsync(CompletePhoneRegistrationRequest request, CancellationToken cancellationToken)
            => await authService.CompletePhoneRegistrationAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<LoginResponse> EmailPasswordLoginAsync(EmailPasswordLoginRequest request, CancellationToken cancellationToken)
            => await authService.EmailPasswordLoginAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<OtpChallengeResponse> SendPhoneLoginOtpAsync(SendPhoneLoginOtpRequest request, CancellationToken cancellationToken)
            => await authService.SendPhoneLoginOtpAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<LoginResponse> PhoneOtpLoginAsync(PhoneOtpLoginRequest request, CancellationToken cancellationToken)
            => await authService.PhoneOtpLoginAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<LoginResponse> LoginWithPasswordAsync(PasswordLoginRequest request, CancellationToken cancellationToken)
            => await authService.PasswordLoginAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken)
            => await authService.SendOtpAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task<LoginResponse> OtpLoginAsync(ValidateOtpRequest request, CancellationToken cancellationToken)
            => await authService.OtpLoginAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
            => await authService.ForgotPasswordAsync(request, cancellationToken);

        [HttpPost("[action]")]
        public async Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpRequest request, CancellationToken cancellationToken)
            => await authService.ResetPasswordWithOtpAsync(request, cancellationToken);

        [HttpGet("[action]")]
        [ApiCustomAuthorize(false, RoleHelper.User, RoleHelper.Admin, RoleHelper.SuperAdmin)]
        public async Task<IReadOnlyList<AccountSessionItemDto>> MySessionsAsync(CancellationToken cancellationToken)
            => await authService.GetMySessionsAsync(cancellationToken);

        [HttpPost("[action]")]
        [ApiCustomAuthorize(false, RoleHelper.User, RoleHelper.Admin, RoleHelper.SuperAdmin)]
        public async Task RevokeSessionAsync(RevokeSessionRequest request, CancellationToken cancellationToken)
            => await authService.RevokeSessionAsync(request, cancellationToken);

        [HttpPost("[action]")]
        [ApiCustomAuthorize(false, RoleHelper.User, RoleHelper.Admin, RoleHelper.SuperAdmin)]
        public async Task RevokeOtherSessionsAsync(CancellationToken cancellationToken)
            => await authService.RevokeOtherSessionsAsync(cancellationToken);

        [HttpGet("[action]")]
        [ApiCustomAuthorize(false, RoleHelper.User, RoleHelper.Admin, RoleHelper.SuperAdmin)]
        public async Task<MfaStatusResponse> MfaStatusAsync(CancellationToken cancellationToken)
            => await authService.GetMfaStatusAsync(cancellationToken);

        [HttpPost("[action]")]
        [ApiCustomAuthorize(false, RoleHelper.User, RoleHelper.Admin, RoleHelper.SuperAdmin)]
        public async Task SetMfaStatusAsync(SetMfaStatusRequest request, CancellationToken cancellationToken)
            => await authService.SetMfaStatusAsync(request, cancellationToken);

        [HttpGet("[action]")]
        [ApiCustomAuthorize(false,
            RoleHelper.User,
            RoleHelper.Admin,
            RoleHelper.SuperAdmin,
            RoleHelper.ThirdParty,
            RoleHelper.FinanceManager,
            RoleHelper.FinancePerformancer,
            RoleHelper.Marketing)]
        public async Task<CurrentUserAuthorizationResponse> MyAuthorizationAsync(CancellationToken cancellationToken)
            => await authService.GetMyAuthorizationAsync(cancellationToken);
    }
}
