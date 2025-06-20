using Library.Core.Dtos.ResponseDto;
using Library.Core.Dtos.UserDto;

namespace Library.Core.Services;

public interface IUserService
{
    Task<ResultService> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<ResultService> DeleteUser(string userId);
    Task<ResultService<UserDto>> GetUserByIdAsync(string userId);
    Task<ResultService<List<UserDto>>> GetAllUsersAsync();
}