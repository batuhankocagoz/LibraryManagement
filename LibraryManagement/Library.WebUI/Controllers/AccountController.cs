using Library.WebUI.Dtos.UserDto;
using Library.WebUI.Services.AccountServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebUI.Controllers;


public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAccountService _accountService;

    public AccountController(IHttpClientFactory httpClientFactory,
                             IHttpContextAccessor contextAccessor,
                             IAccountService accountService)
    {
        _httpClientFactory = httpClientFactory;
        _accountService = accountService;
        _contextAccessor = contextAccessor;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        await _accountService.RegisterUserAsync(registerDto);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        try
        {
            await _accountService.LoginAsync(loginDto);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Email veya Parola hatalı.");
            return View(loginDto);
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var response = await _accountService.GetRentedBooksByUser();
        return View(response.Data);
    }
}
