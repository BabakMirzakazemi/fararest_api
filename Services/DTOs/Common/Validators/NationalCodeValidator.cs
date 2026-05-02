using Common.Configurations;
using FluentValidation;
using FluentValidation.Validators;

namespace Services.DTOs.Common.Validators;

public class NationalCodeValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    public override string Name => nameof(NationalCodeValidator<T, TProperty>);

    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        var result = false;
        var nationalCode = value?.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(nationalCode))
            context.AddFailure(MessageBuilder.CreateNotEmptyErrorMessage(ApplicationPropertyPersianName.NaturalNationalCode));
        else if (nationalCode.Length < ApplicationDefaultSettings.NaturalNationalCodeLength )
            context.AddFailure(MessageBuilder.CreateInvalidLengthErrorMessage(ApplicationPropertyPersianName.NaturalNationalCode));
        else if (nationalCode.Length == ApplicationDefaultSettings.NaturalNationalCodeLength && !nationalCode.IsNaturalNationalCode())
            context.AddFailure(MessageBuilder.CreateInvalidErrorMessage(ApplicationPropertyPersianName.NaturalNationalCode));
        else
            result = true;

        return result;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return string.Empty;
    }
}
