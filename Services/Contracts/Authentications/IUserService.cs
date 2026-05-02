using Microsoft.AspNetCore.Http;
using Services.Contracts.Repositories;
using Services.DTOs.Admins;
using Services.DTOs.Users;
//using Services.DTOs.Users.UpdateEmail;
//using Services.DTOs.Users.UpdatePhoneNumber;

namespace Services.Contracts.Authentications;

public interface IUserService : IRepository<User>
{
    //Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken);

    //Task<UserInfoDTO> GetInfo(CancellationToken cancellationToken);

    //Task<string> UpdateAvatar(IFormFile avatarFile, CancellationToken cancellationToken);

    //Task<string> GetAvatarPath(CancellationToken cancellationToken);

    //Task ChangeNickNameVisibility(CancellationToken cancellationToken);

    //Task UpdateNickNameAsync(UpdateNickNameRequest request, CancellationToken cancellationToken);

    Task<PagingDTO<UserListDTO>> All(FilterUserRequest dto, CancellationToken cancellationToken);
    Task<CursorPagingDTO<UserListDTO>> AllByCursor(FilterUserCursorRequest dto, CancellationToken cancellationToken);

    //Task<UserDetailsDTO> Details(Guid userId, CancellationToken cancellationToken);

    //Task Update(EditUserRequest dto, CancellationToken cancellationToken);

    //Task<List<UserBaseDTO>> GetAllEmails(CancellationToken cancellationToken);

    //Task<List<UserBaseDTO>> GetAllIds(CancellationToken cancellationToken);

    //Task<List<UserBaseDTO>> GetAllWithCompletedRegister(CancellationToken cancellationToken);

    //Task<List<UserBaseDTO>> GetAllWithUnCompletedRegister(CancellationToken cancellationToken);

    //Task<List<UserBaseDTO>> GetUsersWithoutInvestment(CancellationToken cancellationToken);

    //Task<List<UserBaseDTO>> GetUsersWithInvestment(CancellationToken cancellationToken);

    //Task GenerateLoginOtpAsync(GenerateLoginOtpRequest dto, CancellationToken cancellationToken);

    //Task SendOtpForUpdatePasswordAsync(CancellationToken cancellationToken);

    //Task<SendUpdateEmailOtpResponse> SendOtpForUpdateEmailAsync(SendUpdateEmailOtpRequest request, CancellationToken cancellationToken);

    //Task ValidateUpdateEmailOtpAsync(ValidateUpdateEmailOtpRequest request, CancellationToken cancellationToken);

    //Task SendOtpForUpdatePhoneNumberAsync(SendUpdatePhoneNumberOtpRequest request, CancellationToken cancellationToken);

    //Task ValidateUpdatePhoneNumberOtpAsync(ValidateUpdatePhoneNumberOtpRequest request, CancellationToken cancellationToken);
}
