using Common.Configurations;
using FluentValidation;
using Services.Contracts.Authentications;
using Services.DTOs.Common.Users;

namespace Services.DTOs.Common.Validators;

public class UserIdDTOValidator : AbstractValidator<UserIdDTO>
{
    public UserIdDTOValidator(IUserService userService)
    {
        RuleFor(dto => dto.UserId)
            .MustExistAsync((id, cancellationToken) => userService.IsExistAsync(u => u.Id == id, cancellationToken), ApplicationPropertyPersianName.UserId);
    }
}
