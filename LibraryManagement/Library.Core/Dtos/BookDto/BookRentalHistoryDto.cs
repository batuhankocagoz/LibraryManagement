namespace Library.Core.Dtos.BookDto;

public record BookRentalHistoryDto(int BookId, string BookTitle, string ISBN, string RentedBy, string UserId, DateTime RentDate, DateTime? ReturnDate);