namespace Library.Core.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public bool IsAvailable { get; set; }

    public ICollection<Rental> Rentals { get; set; }
}

