using Microsoft.AspNetCore.Identity;

namespace Library.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public bool IsApproved { get; set; } = false;
    public RefreshToken RefreshToken { get; set; }
    public ICollection<Rental> Rentals { get; set; }
}
