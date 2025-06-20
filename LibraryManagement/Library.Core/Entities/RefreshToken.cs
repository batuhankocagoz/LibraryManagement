namespace Library.Core.Entities;
public class RefreshToken
{
    public int Id { get; set; }
    public ApplicationUser User { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
}
