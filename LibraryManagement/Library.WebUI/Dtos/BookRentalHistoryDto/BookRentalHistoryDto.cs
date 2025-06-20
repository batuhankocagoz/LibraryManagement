namespace Library.WebUI.Dtos.BookRentalHistoryDto;

public class BookRentalHistoryDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; }
    public string ISBN { get; set; }
    public string RentedBy { get; set; }
    public string UserId { get; set; }
    public DateTime RentDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
