using Common.Configurations;
using FluentValidation.Validators;
using FluentValidation;

namespace Services.DTOs.Common.Validators;

public class PasswordValidator<T, TProperty> : PropertyValidator<T, TProperty>
{
    private readonly Func<T, string> _repeatPasswordFunc;

    public PasswordValidator(Func<T, string> repeatPasswordFunc)
    {
        _repeatPasswordFunc = repeatPasswordFunc;
    }

    public override string Name => nameof(PasswordValidator<T, TProperty>);

    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        var result = false;
        var password = value?.ToString()?.Trim() ?? string.Empty;
        var repeatPassword = _repeatPasswordFunc(context.InstanceToValidate);

        if (string.IsNullOrEmpty(password))
            context.AddFailure(MessageBuilder.CreateNotEmptyErrorMessage(ApplicationPropertyPersianName.Password));
        else if (!Regex.IsMatch(password, RegexHelper.Password))
            context.AddFailure(MessageBuilder.CreateInvalidErrorMessage(ApplicationPropertyPersianName.Password));
        else if (password != repeatPassword)
            context.AddFailure(MessageBuilder.CreateNotSameErrorMessage(ApplicationPropertyPersianName.Password));
        else
            result = true;

        return result;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return string.Empty;
    }
}
