using System.Net;
using AutoMapper;
using Library.Core.Dtos.RentalDto;
using Library.Core.Dtos.ResponseDto;
using Library.Core.Dtos.UserDto;
using Library.Core.Entities;
using Library.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.Service.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager,
                       IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
        
    }

    public async Task<ResultService> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var user = new ApplicationUser()
        {
            UserName = userRegisterDto.Username,
            FullName = userRegisterDto.FullName,
            Email = userRegisterDto.Email
        };

        var identityResult = await _userManager.CreateAsync(user, userRegisterDto.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors.Select(x => x.Description).ToList();
            return ResultService.Fail(errors);
        }

        return ResultService.Success(HttpStatusCode.Created);
    }

    public async Task<ResultService> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ResultService.Fail("No user found to delete");

        await _userManager.DeleteAsync(user);
        return ResultService.Success(HttpStatusCode.NoContent);
    }

    public async Task<ResultService<List<UserDto>>> GetAllUsersAsync()
    {
        var userListDto = await _userManager.Users
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Email,
                u.IsApproved,
                u.Rentals.Select(br => new BookRentalDto(
                    br.BookId,
                    br.Book.Title,
                    br.Book.ISBN,
                    br.RentDate,
                    br.ReturnDate
                )).ToList()
            ))
            .ToListAsync();

        return ResultService<List<UserDto>>.Succcess(userListDto);
    }

    public async Task<ResultService<UserDto>> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Rentals)
            .ThenInclude(br => br.Book)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return ResultService<UserDto>.Fail("User not found");

        var userDto = new UserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.IsApproved,
            user.Rentals.Select(br => new BookRentalDto(
                br.BookId,
                br.Book.Title,
                br.Book.ISBN,
                br.RentDate,
                br.ReturnDate
            )).ToList()
        );
        return ResultService<UserDto>.Succcess(userDto);
    }


}
