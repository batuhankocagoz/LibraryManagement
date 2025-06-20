namespace Library.Core.Dtos.TokenDto;

public record TokenDto(string AccessToken, DateTime AccessTokenExpiration, string RefreshToken, DateTime RefreshTokenExpiration);