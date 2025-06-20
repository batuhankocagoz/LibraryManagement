using Library.Core.Dtos.RentalDto;

namespace Library.WebUI.Dtos.UserDto;

public class UserDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public bool IsApproved { get; set; }
    public List<BookRentalDto> RentalHistory { get; set; }
}
