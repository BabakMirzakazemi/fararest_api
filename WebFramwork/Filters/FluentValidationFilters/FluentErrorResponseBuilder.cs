using ValidationError = WebFramework.Api.ValidationError;
using ValidationFailure = FluentValidation.Results.ValidationFailure;
namespace WebFramework.Filters.FluentValidationFilters;

public static class FluentErrorResponseBuilder
{
    public static List<ValidationError> BuildValidationErrors(this IEnumerable<ValidationFailure> errors)
    {
        return errors
            .Where(e => !string.IsNullOrEmpty(e.ErrorMessage))
            .Select(e => new ValidationError
            {
                Property = e.PropertyName,
                Message = e.ErrorMessage,
            })
            .ToList();
    }
}
