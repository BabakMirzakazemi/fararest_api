using FluentValidation;

namespace Services.DTOs.Common.Validators;

public static class FluentValidationExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="ruleBuilder"></param>
    /// <param name="propertyName">It can be set in WithName Method of validation</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> ValidatePhoneNumber<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        => ruleBuilder.SetValidator(new PhoneNumberValidator<T, TProperty>());

    public static IRuleBuilderOptions<T, TProperty> ValidateNationalCode<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        => ruleBuilder.SetValidator(new NationalCodeValidator<T, TProperty>());

    public static IRuleBuilderOptions<T, TProperty> ValidatePassword<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<T, string> repeatPasswordFunc)
        => ruleBuilder.SetValidator(new PasswordValidator<T, TProperty>(repeatPasswordFunc));

    public static IRuleBuilderOptions<T, TProperty> NotNullAndEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string propertyName)
    {
        return ruleBuilder
            .NotNull()
            .WithName(propertyName)
            .WithMessage(MessageBuilder.CreateNotNullErrorMessage("{PropertyName}"))
            .NotEmpty()
            .WithMessage(MessageBuilder.CreateNotEmptyErrorMessage("{PropertyName}"));
    }

    public static IRuleBuilderOptions<T, TProperty> MustExistAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, CancellationToken, Task<bool>> predicate, string displayName)
    {
        return ruleBuilder
            .MustAsync(async (property, cancellationToken) => await predicate(property, cancellationToken))
            .WithMessage(MessageBuilder.CreateNotFoundErrorMessage(displayName));
    }

    public static IRuleBuilderOptions<T, TProperty> MustNotExistAsync<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, CancellationToken, Task<bool>> predicate, string displayName)
    {
        return ruleBuilder
            .MustAsync(async (property, cancellationToken) => !await predicate(property, cancellationToken))
            .WithMessage(MessageBuilder.CreateDuplicateErrorMessage(displayName));
    }

    public static Type? GetRequestType(this IValidator validator)
        => validator.GetType().BaseType?.GetGenericArguments().FirstOrDefault();
}
