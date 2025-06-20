using Library.WebUI.Dtos.UserDto;
using Library.WebUI.Services.RoleServices;
using Library.WebUI.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebUI.Controllers;


public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserController(IUserService userService,
                          IHttpContextAccessor contextAccessor)
    {
        _userService = userService;
        _contextAccessor = contextAccessor;
    }

    public async Task<IActionResult> GetAllUserList()
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Account");
        }

        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            return RedirectToAction("Index", "Home");
        }

        var response = await _userService.GetUserListAsync();
        var userList = response.Data ?? new List<UserDto>(); 
        return View(userList);
    }


    public async Task<IActionResult> GetUserById(string id)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Account");
        }
        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            return RedirectToAction("Index", "Home");
        }
        var response = await _userService.GetUserByIdAsync(id);
        return View(response.Data);
    }

    public async Task<IActionResult> ApproveUser(string userId)
    {
        await _userService.ApproveUserAsync(userId);
        return RedirectToAction("GetAllUserList");
    }

    [HttpPost]
    public async Task<IActionResult> AssignToManagerRole(string userId)
    {
        await _userService.AssignToManagerRoleAsync(userId);
        return RedirectToAction("GetAllUserList");
    }

    public async Task<IActionResult> AssignToAdminRole(string userId)
    {
        await _userService.AssignToAdminRoleAsync(userId);
        return RedirectToAction("GetAllUserList");
    }
}