using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Library.WebUI.Dtos;
using Library.WebUI.Dtos.BookDto;
using Library.WebUI.Dtos.TokenDto;
using Library.WebUI.Dtos.UserDto;
using Library.WebUI.Services.JwtServices;
using Newtonsoft.Json;

namespace Library.WebUI.Services.AccountServices;

public class AccountService : IAccountService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor; 

    public AccountService(IHttpClientFactory httpClientFactory, 
        IHttpContextAccessor contextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _contextAccessor = contextAccessor;
    }

    public async Task RegisterUserAsync(RegisterDto registerDto)
    {
        var client = _httpClientFactory.CreateClient();
        var jsonData = JsonConvert.SerializeObject(registerDto);
        StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://localhost:7282/api/Users", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"User registration failed. StatusCode: {response.StatusCode}, Details: {errorMessage}");
        }
    }

    public async Task LoginAsync(LoginDto loginDto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("https://localhost:7282/api/Auths/CreateToken", loginDto);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Login failed");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenDto>>();
        _contextAccessor.HttpContext.Session.SetString("token", result.Data.AccessToken);
        _contextAccessor.HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken);
    }

    public async Task<ApiResponse<List<BookDto>>> GetRentedBooksByUser()
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return new ApiResponse<List<BookDto>> { ErrorMessage = "Access token not found in the session" };
        }

        var userId = JwtHelper.GetClaimValue(accessToken, ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return new ApiResponse<List<BookDto>> { ErrorMessage = "User Id not found in the token" };
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetFromJsonAsync<ApiResponse<List<BookDto>>>
            ("https://localhost:7282/api/Books/GetRentedBooksByUser?userId=" + userId);

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            return new ApiResponse<List<BookDto>> { ErrorMessage = response.ErrorMessage };
        }

        return response;
    }

    public async Task LogoutAsync()
    {
        var token = _contextAccessor.HttpContext.Session.GetString("token");
        var userId = JwtHelper.GetClaimValue(token, ClaimTypes.NameIdentifier);
        await RevokeRefreshTokenAsync(userId);
        _contextAccessor.HttpContext.Session.Remove("token");
        _contextAccessor.HttpContext.Session.Remove("RefreshToken");
    }

    public async Task RevokeRefreshTokenAsync(string userId)
    {
        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        var response = await client.DeleteAsync("https://localhost:7282/api/Auths/RevokeRefreshToken/" + userId);
        if (response is null)
        {
            throw new Exception("api response is null");
        }
    }

}
