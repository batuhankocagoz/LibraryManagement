using Library.WebUI.Dtos.BookDto;
using Library.WebUI.Dtos.BookRentalHistoryDto;
using Library.WebUI.Services.BookServices;
using Library.WebUI.Services.RoleServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebUI.Controllers;


public class BookController : Controller
{
    private readonly IBookService _bookService;
    private readonly IHttpContextAccessor _contextAccessor;

    public BookController(IBookService bookService, IHttpContextAccessor contextAccessor)
    {
        _bookService = bookService;
        _contextAccessor = contextAccessor;
    }

    public async Task<IActionResult> GetAllBooks()
    {
        var response = await _bookService.GetAllBooksAsync();
        return View(response.Data);
    }

    public async Task<IActionResult> RentBook(int id)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Account");
        }
        var response = await _bookService.RentBookAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            TempData["RentBookError"] = response.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        TempData["RentBookSuccess"] = "Kitap başarıyla kiralandı";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> ReturnBook(int id)
    {
        var response = await _bookService.ReturnBookAsync(id);
        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            return RedirectToAction("Profile", "Account");
        }

        TempData["ReturnBookSuccess"] = "Kitap başarıyla iade edildi";
        return RedirectToAction("Profile", "Account");
    }

    public IActionResult CreateBook()
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Account");
        }
        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook(CreateBookRequest request)
    {
        await _bookService.CreateBook(request);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> DeleteBook(int id)
    {
        await _bookService.DeleteBookAsync(id);
        return RedirectToAction("GetAllBooks", "Book");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateBook(int bookId)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Account");
        }
        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            return RedirectToAction("Index", "Home");
        }
        var book = await _bookService.GetBookById(bookId);
        return View(book.Data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBook(int bookId, UpdateBookRequest request)
    {
        await _bookService.UpdateBook(bookId, request);
        return RedirectToAction("GetAllBooks", "Book");
    }

    public async Task<IActionResult> GetBookRentalHistory(int id)
    {
        var accessToken = _contextAccessor.HttpContext.Session.GetString("token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Account");
        }
        var roles = GetUserRoles.GetRolesFromToken(accessToken);
        if (!roles.Contains("admin") && !roles.Contains("manager"))
        {
            return RedirectToAction("Index", "Home");
        }
        var response = await _bookService.GetBookRentalHistoryById(id);

        if (response.Data == null || !response.Data.Any())
        {
            ViewBag.NoHistoryMessage = "Bu kitap için kiralama geçmişi bulunmamaktadır.";
            return View(new List<BookRentalHistoryDto>());
        }

        ViewBag.BookTitle = response.Data.First().BookTitle;
        return View(response.Data);
    }
}
