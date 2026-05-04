using Services.DTOs.Accounts.CompleteRegistration;
using Services.DTOs.Accounts.Login;
using Services.DTOs.Accounts.Mfa;
using Services.DTOs.Accounts.Otp;
using Services.DTOs.Accounts.Recovery;
using Services.DTOs.Accounts.Registration;
using Services.DTOs.Accounts.SendOtp;
using Services.DTOs.Accounts.Sessions;
using Services.DTOs.Accounts.StartAuthentication;
using Services.DTOs.Accounts.ValidateOtp;
using Services.DTOs.Users.UpdatePassword;

namespace Services.Contracts.Authentications;

public interface IAuthenticationService
{
    //Task<StartAuthenticationResponse> StartAuthenticationAsync(StartAuthenticationRequest request, CancellationToken cancellationToken);

    Task<LoginResponse> PasswordLoginAsync(PasswordLoginRequest request, CancellationToken cancellationToken);

    Task<LoginResponse> OtpLoginAsync(ValidateOtpRequest request, CancellationToken cancellationToken);

    Task<OtpChallengeResponse> RegisterWithEmailAsync(RegisterWithEmailRequest request, CancellationToken cancellationToken);

    Task ConfirmEmailRegistrationAsync(ConfirmEmailRegistrationRequest request, CancellationToken cancellationToken);
    
    Task<OtpChallengeResponse> ResendEmailActivationLinkAsync(ResendEmailActivationLinkRequest request, CancellationToken cancellationToken);

    Task<OtpChallengeResponse> StartPhoneRegistrationAsync(StartPhoneRegistrationRequest request, CancellationToken cancellationToken);

    Task<OtpChallengeResponse> ResendPhoneRegistrationOtpAsync(ResendPhoneRegistrationOtpRequest request, CancellationToken cancellationToken);

    Task CompletePhoneRegistrationAsync(CompletePhoneRegistrationRequest request, CancellationToken cancellationToken);

    Task<LoginResponse> EmailPasswordLoginAsync(EmailPasswordLoginRequest request, CancellationToken cancellationToken);

    Task<OtpChallengeResponse> SendPhoneLoginOtpAsync(SendPhoneLoginOtpRequest request, CancellationToken cancellationToken);

    Task<LoginResponse> PhoneOtpLoginAsync(PhoneOtpLoginRequest request, CancellationToken cancellationToken);

    //Task<LoginResponse> AdminLoginAsync(AdminLoginRequest request, CancellationToken cancellationToken);

    Task<SendOtpResponse> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken);

    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken);

    Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyList<AccountSessionItemDto>> GetMySessionsAsync(CancellationToken cancellationToken);

    Task RevokeSessionAsync(RevokeSessionRequest request, CancellationToken cancellationToken);

    Task RevokeOtherSessionsAsync(CancellationToken cancellationToken);

    Task<MfaStatusResponse> GetMfaStatusAsync(CancellationToken cancellationToken);

    Task SetMfaStatusAsync(SetMfaStatusRequest request, CancellationToken cancellationToken);

    //Task ValidateCompleteRegistrationDataAsync(CompleteUserRegistrationRequest request, CancellationToken cancellationToken);

    Task SignOutAsync(CancellationToken cancellationToken);

    Task UpdatePasswordAsync(UpdatePasswordRequest input, CancellationToken cancellationToken);
}
