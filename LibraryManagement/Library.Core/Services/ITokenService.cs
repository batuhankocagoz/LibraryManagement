using Library.Core.Dtos.TokenDto;
using Library.Core.Entities;

namespace Library.Core.Services;

public interface ITokenService
{
    TokenDto CreateToken(ApplicationUser user);
}
