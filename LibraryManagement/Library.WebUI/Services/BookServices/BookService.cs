﻿using System.Net.Http.Headers;
using System.Security.Claims;
using Library.WebUI.Dtos;
using Library.WebUI.Dtos.BookDto;
using Library.WebUI.Dtos.BookRentalHistoryDto;
using Library.WebUI.Services.JwtServices;
using Library.WebUI.Services.RoleServices;

namespace Library.WebUI.Services.BookServices;

public class BookService : IBookService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public BookService(IHttpClientFactory httpClientFactory, 
        IHttpContextAccessor contextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _contextAccessor = contextAccessor;
    }

    public async Task<ApiResponse<List<BookDto>>> GetAllBooksAsync()
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Access token not found");
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            throw new Exception("Not authorized");
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.GetFromJsonAsync<ApiResponse<List<BookDto>>>("https://localhost:7282/api/Books");

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception($"API request failed: {response.ErrorMessage}");
            }
            return response;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"API request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BookDto>>> GetAvailableBooks()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetFromJsonAsync<ApiResponse<List<BookDto>>>("https://localhost:7282/api/Books/GetAvailableBooks");

        if (response is null)
        {
            throw new Exception("api response is null");
        }

        return response;
    }

    public async Task<ApiResponse<BookDto>> GetBookById(int bookId)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return new ApiResponse<BookDto> { ErrorMessage = "Access token not found in the session" };
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            return new ApiResponse<BookDto> { ErrorMessage = "Not authorized" };
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.GetFromJsonAsync<ApiResponse<BookDto>>("https://localhost:7282/api/Books/" + bookId);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new ApiResponse<BookDto> { ErrorMessage = response.ErrorMessage };
            }
            return response;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"API request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse> RentBookAsync(int bookId)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return new ApiResponse { ErrorMessage = "Access token not found in the session" };
        }

        var userId = JwtHelper.GetClaimValue(accessToken, ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return new ApiResponse { ErrorMessage = "User Id not found in the token" };
        }
        var rentBookRequest = new RentBookRequest
        {
            BookId = bookId,
            UserId = userId
        };

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PostAsJsonAsync("https://localhost:7282/api/Books/RentBook", rentBookRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ApiResponse { ErrorMessage = errorMessage };
        }

        return await response.Content.ReadFromJsonAsync<ApiResponse>();
    }

    public async Task<ApiResponse> ReturnBookAsync(int bookId)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return new ApiResponse { ErrorMessage = "Access token not found in the session" };
        }

        var userId = JwtHelper.GetClaimValue(accessToken, ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return new ApiResponse { ErrorMessage = "User Id not found in the token" };
        }

        var returnBookRequest = new ReturnBookRequest
        {
            BookId = bookId,
            UserId = userId
        };

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.PostAsJsonAsync("https://localhost:7282/api/Books/ReturnBook", returnBookRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ApiResponse { ErrorMessage = errorMessage };
        }
        return new ApiResponse() { };
    }

    public async Task CreateBook(CreateBookRequest request)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Access token not found");
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            throw new Exception("Not authorized");
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.PostAsJsonAsync("https://localhost:7282/api/Books", request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API request failed: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"API request failed: {ex.Message}");
        }
    }

    public async Task UpdateBook(int bookId, UpdateBookRequest request)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Access token not found");
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            throw new Exception("Not authorized");
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.PutAsJsonAsync("https://localhost:7282/api/Books/" + bookId, request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API request failed: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"API request failed: {ex.Message}");
        }
    }

    public async Task DeleteBookAsync(int bookId)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Access token not found");
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            throw new Exception("Not authorized");
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await client.DeleteAsync("https://localhost:7282/api/Books/" + bookId);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API request failed: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"API request failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<BookRentalHistoryDto>>> GetBookRentalHistoryById(int bookId)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Access token not found");
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            throw new Exception("Not authorized");
        }

        var client = _httpClientFactory.CreateClient("AuthorizeClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetFromJsonAsync<ApiResponse<List<BookRentalHistoryDto>>>("https://localhost:7282/api/Books/GetBookRentalHistory/" + bookId);

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            return new ApiResponse<List<BookRentalHistoryDto>>() { ErrorMessage = response.ErrorMessage };
        }
        return response;

    }
}
