using Common.Configurations;
using FluentValidation;
using Services.Contracts.Authentications;
using Services.DTOs.Accounts.Login;
using Services.DTOs.Accounts.Registration;

namespace Services.DTOs.Accounts;

public class RegisterWithEmailRequestValidator : AbstractValidator<RegisterWithEmailRequest>
{
    public RegisterWithEmailRequestValidator(IUserService userService)
    {
        RuleFor(dto => dto.Email)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Email)
            .Must(email => Regex.IsMatch(email.Trim(), $"^{RegexHelper.Email}$"))
            .WithMessage(ApplicationMessages.InvalidEmail)
            .MustNotExistAsync((email, cancellationToken) =>
                userService.IsExistAsync(u => u.Email != null && u.Email.ToUpper() == email.Trim().ToUpper(), cancellationToken),
                ApplicationPropertyPersianName.Email);

        RuleFor(dto => dto.Password)
            .ValidatePassword(dto => dto.ConfirmPassword);

        RuleFor(dto => dto.FullName)
            .MaximumLength(200)
            .When(dto => !string.IsNullOrWhiteSpace(dto.FullName))
            .WithMessage(MessageBuilder.CreateInvalidLengthErrorMessage("200"));
    }
}

public class ConfirmEmailRegistrationRequestValidator : AbstractValidator<ConfirmEmailRegistrationRequest>
{
    public ConfirmEmailRegistrationRequestValidator()
    {
        RuleFor(dto => dto.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage(MessageBuilder.CreateNotEmptyErrorMessage(ApplicationPropertyPersianName.UserId));

        RuleFor(dto => dto.Email)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Email)
            .Must(email => Regex.IsMatch(email.Trim(), $"^{RegexHelper.Email}$"))
            .WithMessage(ApplicationMessages.InvalidEmail);

        RuleFor(dto => dto.Otp)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Otp)
            .Must(otp => Regex.IsMatch(otp.Trim(), "^\\d{6}$"))
            .WithMessage(ApplicationMessages.InvalidOtp);
    }
}

public class StartPhoneRegistrationRequestValidator : AbstractValidator<StartPhoneRegistrationRequest>
{
    public StartPhoneRegistrationRequestValidator(IUserService userService)
    {
        RuleFor(dto => dto.Mobile)
            .ValidatePhoneNumber()
            .MustNotExistAsync((mobile, cancellationToken) =>
                userService.IsExistAsync(u => u.Mobile != null && u.Mobile == mobile.Trim(), cancellationToken),
                ApplicationPropertyPersianName.PhoneNumber);

        RuleFor(dto => dto.FullName)
            .MaximumLength(200)
            .When(dto => !string.IsNullOrWhiteSpace(dto.FullName))
            .WithMessage(MessageBuilder.CreateInvalidLengthErrorMessage("200"));
    }
}

public class ResendEmailActivationLinkRequestValidator : AbstractValidator<ResendEmailActivationLinkRequest>
{
    public ResendEmailActivationLinkRequestValidator()
    {
        RuleFor(dto => dto.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage(MessageBuilder.CreateNotEmptyErrorMessage(ApplicationPropertyPersianName.UserId));

        RuleFor(dto => dto.Email)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Email)
            .Must(email => Regex.IsMatch(email.Trim(), $"^{RegexHelper.Email}$"))
            .WithMessage(ApplicationMessages.InvalidEmail);
    }
}

public class CompletePhoneRegistrationRequestValidator : AbstractValidator<CompletePhoneRegistrationRequest>
{
    public CompletePhoneRegistrationRequestValidator()
    {
        RuleFor(dto => dto.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage(MessageBuilder.CreateNotEmptyErrorMessage(ApplicationPropertyPersianName.UserId));

        RuleFor(dto => dto.Mobile)
            .ValidatePhoneNumber();

        RuleFor(dto => dto.Otp)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Otp)
            .Must(otp => Regex.IsMatch(otp.Trim(), "^\\d{6}$"))
            .WithMessage(ApplicationMessages.InvalidOtp);
    }
}

public class EmailPasswordLoginRequestValidator : AbstractValidator<EmailPasswordLoginRequest>
{
    public EmailPasswordLoginRequestValidator()
    {
        RuleFor(dto => dto.Email)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Email)
            .Must(email => Regex.IsMatch(email.Trim(), $"^{RegexHelper.Email}$"))
            .WithMessage(ApplicationMessages.InvalidEmail);

        RuleFor(dto => dto.Password)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Password);
    }
}

public class SendPhoneLoginOtpRequestValidator : AbstractValidator<SendPhoneLoginOtpRequest>
{
    public SendPhoneLoginOtpRequestValidator()
    {
        RuleFor(dto => dto.Mobile)
            .ValidatePhoneNumber();
    }
}

public class PhoneOtpLoginRequestValidator : AbstractValidator<PhoneOtpLoginRequest>
{
    public PhoneOtpLoginRequestValidator()
    {
        RuleFor(dto => dto.Mobile)
            .ValidatePhoneNumber();

        RuleFor(dto => dto.Otp)
            .NotNullAndEmpty(ApplicationPropertyPersianName.Otp)
            .Must(otp => Regex.IsMatch(otp.Trim(), "^\\d{6}$"))
            .WithMessage(ApplicationMessages.InvalidOtp);
    }
}
