﻿using Library.Core.Dtos.BookDto;
using Library.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : CustomBaseController
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> GetAllBooks()
    {
        var result = await _bookService.GetAllBooksAsync();
        return CustomActionResult(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var result = await _bookService.GetBookByIdAsync(id);
        return CustomActionResult(result);
    }

    [HttpGet("GetAvailableBooks")]
    public async Task<IActionResult> GetAvailableBooks()
    {
        var result = await _bookService.GetAvaliableBooksAsync();
        return CustomActionResult(result);
    }

    [HttpGet("GetBookRentalHistory/{bookId:int}")]
    [Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> GetBookRentalHistory(int bookId)
    {
        var result = await _bookService.GetBookRentalHistoryAsync(bookId);
        return CustomActionResult(result);
    }

    [HttpGet("GetRentedBooksByUser")]
    [Authorize]
        public async Task<IActionResult> GetRentedBooksByUser(string userId)
    {
        var result = await _bookService.RentedBooksByUser(userId);
        return CustomActionResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> CreateBook(BookCreateUpdateDto request)
    {
        var result = await _bookService.AddBookAsync(request);
        return CustomActionResult(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBook(int id, BookCreateUpdateDto request)
    {
        var result = await _bookService.UpdateBookAsync(id, request);
        return CustomActionResult(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin, manager")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var result = await _bookService.DeleteBookAsync(id);
        return CustomActionResult(result);
    }

    [HttpPost("RentBook")]
    [Authorize]
    public async Task<IActionResult> RentBook([FromBody] RentBookRequest request)
    {
        var result = await _bookService.RentBookAsync(request.BookId, request.UserId);
        return CustomActionResult(result);
    }


    [HttpPost("ReturnBook")]
    [Authorize]
    public async Task<IActionResult> ReturnBook(ReturnBookRequest request)
    {
        var result = await _bookService.ReturnBookAsync(request.BookId, request.UserId);
        return CustomActionResult(result);
    }

}