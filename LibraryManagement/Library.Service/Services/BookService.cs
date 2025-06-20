using System.Net;
using AutoMapper;
using Library.Core.Dtos.BookDto;
using Library.Core.Dtos.ResponseDto;
using Library.Core.Entities;
using Library.Core.Repositories;
using Library.Core.Services;
using Library.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.Service.Services;

public class BookService : IBookService
{
    private readonly IGenericRepository<Book> _bookRepository;
    private readonly IGenericRepository<Rental> _rentalRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IGenericRepository<Book> bookRepository,
                       UserManager<ApplicationUser> userManager,
                       IMapper mapper,
                       IUnitOfWork unitOfWork,
                       IGenericRepository<Rental> rentalRepository)
    {
        _bookRepository = bookRepository;
        _userManager = userManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _rentalRepository = rentalRepository;
    }

    public async Task<ResultService> AddBookAsync(BookCreateUpdateDto createUpdateBookDto)
    {
        var bookExist = await _bookRepository.Where(b => b.ISBN == createUpdateBookDto.ISBN).AnyAsync();

        if (bookExist)
            return ResultService.Fail("This ISBN number already used");

        var book = _mapper.Map<Book>(createUpdateBookDto);

        book.IsAvailable = true;

        await _bookRepository.CreateAsync(book);
        await _unitOfWork.SaveChangesAsync();

        return ResultService.Success();
    }


    public async Task<ResultService> DeleteBookAsync(int bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
            return ResultService.Fail("No books found to delete", HttpStatusCode.NotFound);

        var rentals = await _rentalRepository.Where(br => br.BookId == bookId).ToListAsync();
        foreach (var rental in rentals)
        {
            _rentalRepository.Delete(rental);
        }

        _bookRepository.Delete(book);
        await _unitOfWork.SaveChangesAsync();
        return ResultService.Success(HttpStatusCode.NoContent);
    }

    public async Task<ResultService<List<BookDto>>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAll().ToListAsync();
        var booksAsDto = _mapper.Map<List<BookDto>>(books);
        return ResultService<List<BookDto>>.Succcess(booksAsDto);
    }

    public async Task<ResultService<List<BookDto>>> GetAvaliableBooksAsync()
    {
        var availableBooks = await _bookRepository
            .Where(b => b.IsAvailable).ToListAsync();

        var availableBooksAsDto = _mapper.Map<List<BookDto>>(availableBooks);

        // Boş bile olsa success döndür
        return ResultService<List<BookDto>>.Succcess(availableBooksAsDto);
    }


    public async Task<ResultService<BookDto>> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
            return ResultService<BookDto>.Fail("Book not found", HttpStatusCode.NotFound);
        var bookAsDto = _mapper.Map<BookDto>(book);
        return ResultService<BookDto>.Succcess(bookAsDto);
    }

    public async Task<ResultService<List<BookRentalHistoryDto>>> GetBookRentalHistoryAsync(int bookId)
    {
        var rentalHistory = await _rentalRepository
            .Where(br => br.BookId == bookId)
            .Include(br => br.User)
            .Include(br => br.Book)
            .ToListAsync();

        if (!rentalHistory.Any())
            return ResultService<List<BookRentalHistoryDto>>.Succcess(new List<BookRentalHistoryDto>());

        var rentalHistoryDtos = rentalHistory.Select(x => new BookRentalHistoryDto
            (
                x.BookId,
                x.Book.Title,
                x.Book.ISBN,
                x.User.FullName,
                x.UserId,
                x.RentDate,
                x.ReturnDate
            )
        ).ToList();
        return ResultService<List<BookRentalHistoryDto>>.Succcess(rentalHistoryDtos);
    }

    public async Task<ResultService> UpdateBookAsync(int bookId, BookCreateUpdateDto createUpdateBookDto)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
            return ResultService.Fail("No Book found to update", HttpStatusCode.NotFound);

        bool isIsbnExist = await _bookRepository
            .Where(b => b.ISBN == createUpdateBookDto.ISBN && book.ISBN != b.ISBN)
            .AnyAsync();
        if (isIsbnExist)
            return ResultService.Fail("This ISBN number already in use");

        book = _mapper.Map(createUpdateBookDto, book);
        _bookRepository.Update(book);
        await _unitOfWork.SaveChangesAsync();
        return ResultService.Success(HttpStatusCode.NoContent);
    }

    public async Task<ResultService> RentBookAsync(int bookId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ResultService.Fail("User not found");

        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
            return ResultService.Fail("Book not found");

        if (!book.IsAvailable)
            return ResultService.Fail("Book already rented");

        var rental = new Rental()
        {
            BookId = bookId,
            UserId = userId,
            RentDate = DateTime.Now,
            ReturnDate = null
        };

        await _rentalRepository.CreateAsync(rental);

        book.IsAvailable = false;
        _bookRepository.Update(book);

        await _unitOfWork.SaveChangesAsync();
        return ResultService.Success();
    }

    public async Task<ResultService<List<BookDto>>> RentedBooksByUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ResultService<List<BookDto>>.Fail("User not found");

        var rentedBooksByUser = await _rentalRepository
            .Where(br => br.UserId == user.Id && br.ReturnDate == null)
            .Select(x => new BookDto
            (
                x.BookId,
                x.Book.Title,
                x.Book.Author,
                x.Book.ISBN,
                x.Book.IsAvailable
            ))
            .ToListAsync();

        return ResultService<List<BookDto>>.Succcess(rentedBooksByUser);
    }

    public async Task<ResultService> ReturnBookAsync(int bookId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ResultService.Fail("User not found");

        if (user.IsApproved is false)
        {
            return ResultService.Fail("Admin has not approved the membership yet.");
        }

        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book is null)
            return ResultService.Fail("book not found");

        var bookRental = await _rentalRepository
            .Where(br => br.BookId == book.Id &&
                         br.UserId == userId &&
                         br.ReturnDate == null)
            .OrderByDescending(br => br.RentDate)
            .FirstOrDefaultAsync();
        if (bookRental is null)
        {
            return ResultService.Fail("Book Rental history not found");
        }

        bookRental.ReturnDate = DateTime.Now; ;
        book.IsAvailable = true;
        _bookRepository.Update(book);
        _rentalRepository.Update(bookRental);
        await _unitOfWork.SaveChangesAsync();

        return ResultService.Success(HttpStatusCode.NoContent);
    }


}
