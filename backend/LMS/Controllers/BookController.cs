using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        [Authorize(Roles = "SUPER_USER,NORMAL_USER")]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1, [FromQuery] string? searchTerm = null, [FromQuery] int categoryId = 0)
        {
            var books = await _bookService.GetAllBookAsync(pageSize, pageIndex, searchTerm, categoryId);
            return Ok(ApiResponse<PaginatedList<BookResponse>>.Success(books, "Books retrieved successfully."));
        }
        [Authorize("SUPER_USER")]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookRequest bookRequest)
        {
            await _bookService.AddBookAsync(bookRequest);
            return Created(string.Empty, null);
        }
        [Authorize("SUPER_USER")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateRequest bookRequest)
        {
            await _bookService.UpdateBookAsync(id, bookRequest);
            return NoContent();
        }
        [Authorize("SUPER_USER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
        [Authorize(Roles = "SUPER_USER,NORMAL_USER")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            return Ok(ApiResponse<BookDto>.Success(book, "Book retrieved successfully."));
        }
    }
}
