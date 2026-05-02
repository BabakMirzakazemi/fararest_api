using Microsoft.AspNetCore.Identity;

namespace Services.DataInitializer;

public class UserDataInitializer : IDataInitializer
{
    private readonly UserManager<User> _userManager;

    public UserDataInitializer(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public int Order => 2;

    public void InitializeData()
    {
        var user = _userManager.FindByIdAsync(DataInitializerDefaultValues.UserId).GetAwaiter().GetResult();
        if (user == null)
        {
            var userId = Guid.Parse(DataInitializerDefaultValues.UserId);
            user = new User
            {
                Id = userId,
                Email = DataInitializerDefaultValues.Email,
                UserName = DataInitializerDefaultValues.UserName,
                IsActive = true,
                CreatedDate = DateTime.Now,
                FullName = DataInitializerDefaultValues.FullName,
                IsDeleted = false,
                Mobile = DataInitializerDefaultValues.PhoneNumber,
                ConfirmationCode = new ConfirmationCode()
            };

            var createUserResult = _userManager.CreateAsync(user, DataInitializerDefaultValues.Password).GetAwaiter().GetResult();
            if (!createUserResult.Succeeded)
            {
                var createErrors = string.Join(" | ", createUserResult.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"User seed failed for '{user.UserName}'. {createErrors}");
            }
        }

        var roleNamesToAdd = new[] { RoleHelper.User, RoleHelper.Admin, RoleHelper.SuperAdmin };
        var existingRoles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
        var missingRoles = roleNamesToAdd.Except(existingRoles).ToList();

        if (missingRoles.Count > 0)
        {
            var addRolesResult = _userManager.AddToRolesAsync(user, missingRoles).GetAwaiter().GetResult();
            if (!addRolesResult.Succeeded)
            {
                var roleErrors = string.Join(" | ", addRolesResult.Errors.Select(e => $"{e.Code}:{e.Description}"));
                throw new InvalidOperationException($"Assigning roles to seed user failed. {roleErrors}");
            }
        }
    }
}
