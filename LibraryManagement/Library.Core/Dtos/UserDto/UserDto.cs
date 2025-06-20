
using Library.Core.Dtos.RentalDto;

namespace Library.Core.Dtos.UserDto;

public record UserDto(string Id, string FullName, string Email, bool IsApproved, List<BookRentalDto> RentalHistory);