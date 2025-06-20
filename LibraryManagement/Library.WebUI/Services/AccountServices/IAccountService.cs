using Library.WebUI.Dtos;
using Library.WebUI.Dtos.BookDto;
using Library.WebUI.Dtos.UserDto;

namespace Library.WebUI.Services.AccountServices;

public interface IAccountService
{
    Task RegisterUserAsync(RegisterDto registerDto);
    Task LoginAsync(LoginDto loginDto);
    Task LogoutAsync();
    Task<ApiResponse<List<BookDto>>> GetRentedBooksByUser();
    Task RevokeRefreshTokenAsync(string userId);
}
