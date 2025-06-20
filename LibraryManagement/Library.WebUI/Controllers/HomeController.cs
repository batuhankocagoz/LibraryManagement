using System.Diagnostics;
using Library.WebUI.Models;
using Library.WebUI.Services.BookServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebUI.Controllers;


public class HomeController : Controller
{

    private readonly ILogger<HomeController> _logger;
    private readonly IBookService _bookService;

    public HomeController(ILogger<HomeController> logger, IBookService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var bookResponse = await _bookService.GetAvailableBooks();
        if (bookResponse is not null)
        {
            return View(bookResponse.Data);
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}