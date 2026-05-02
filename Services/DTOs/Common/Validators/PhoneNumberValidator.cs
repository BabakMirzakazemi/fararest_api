using Common.Configurations;
using FluentValidation;
using FluentValidation.Validators;

namespace Services.DTOs.Common.Validators;

public class PhoneNumberValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    public override string Name => nameof(PhoneNumberValidator<T, TProperty>);

    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        var result = true;
        var phoneNumber = value?.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(phoneNumber))
        {
            context.AddFailure(MessageBuilder.CreateNotEmptyErrorMessage(ApplicationPropertyPersianName.PhoneNumber));
            result = false;
        }
        else if (!Regex.IsMatch(phoneNumber, RegexHelper.Mobile))
        {
            context.AddFailure(MessageBuilder.CreateInvalidErrorMessage(ApplicationPropertyPersianName.PhoneNumber));
            result = false;
        }

        return result;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return string.Empty;
    }
}
