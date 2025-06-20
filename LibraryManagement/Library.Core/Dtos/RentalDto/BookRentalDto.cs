namespace Library.Core.Dtos.RentalDto;

public record BookRentalDto(int BookId, string BookTitle, string ISBN, DateTime RentDate, DateTime? ReturnDate);

