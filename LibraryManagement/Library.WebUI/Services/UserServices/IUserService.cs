using Library.WebUI.Dtos;
using Library.WebUI.Dtos.UserDto;

namespace Library.WebUI.Services.UserServices;

public interface IUserService
{
    Task<ApiResponse<List<UserDto>>> GetUserListAsync();
    Task<ApiResponse<UserDto>> GetUserByIdAsync(string id);
    Task ApproveUserAsync(string userId);
    Task AssignToManagerRoleAsync(string userId);
    Task AssignToAdminRoleAsync(string userId);
}
