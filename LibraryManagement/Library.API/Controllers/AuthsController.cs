﻿using Library.Api.Controllers;
using Library.Core.Dtos.TokenDto;
using Library.Core.Dtos.UserDto;
using Library.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers;

public class AuthsController : CustomBaseController
{
    private readonly IAuthService _authService;

    public AuthsController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("CreateToken")]
    public async Task<IActionResult> CreateToken(LoginDto request)
    {
        var result = await _authService.CreateTokenAsync(request);
        return CustomActionResult(result);
    }

    [HttpPost("CreateTokenByRefreshToken")]
    public async Task<IActionResult> CreateTokenByRefreshToken(CreateTokenByRefreshTokenRequest request)
    {
        var result = await _authService.CreateTokenByRefreshToken(request.Token);
        return CustomActionResult(result);
    }

    [HttpDelete("RevokeRefreshToken/{userId}")]
    public async Task<IActionResult> RevokeRefreshToken(string userId)
    {
        var result = await _authService.RevokeRefreshToken(userId);
        return CustomActionResult(result);
    }

    [HttpPatch("ApproveUser/{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ApproveUser(string userId)
    {
        var result = await _authService.ApproveUser(userId);
        return CustomActionResult(result);
    }

    [HttpPost("AssignToAdminRole/{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignToAdminRole(string userId)
    {
        var result = await _authService.AssignToAdminRole(userId);
        return CustomActionResult(result);
    }

    [HttpPost("AssignToManagerRole/{userId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignToManagerRole(string userId)
    {
        var result = await _authService.AssignToManagerRole(userId);
        return CustomActionResult(result);
    }
}
