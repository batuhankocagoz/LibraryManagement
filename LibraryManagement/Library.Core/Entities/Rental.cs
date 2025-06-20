namespace Library.Core.Entities;

public class Rental
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public DateTime RentDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}

