using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowBookController : ControllerBase
    {
        private readonly IBookBorrowingRequestService _bookBorrowingRequestService;

        public BorrowBookController(IBookBorrowingRequestService bookBorrowingRequestService)
        {
            _bookBorrowingRequestService = bookBorrowingRequestService;
        }
        [Authorize(Roles ="SUPER_USER,NORMAL_USER")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBookBorrowingRequest([FromBody] BookBorrowingRequestDtoCreate bookBorrowingRequest)
        {
            await _bookBorrowingRequestService.CreateBookBorrowingRequestAsync(bookBorrowingRequest);
            return Ok(ApiResponse<string>.Success("Book borrowing request created successfully."));
        }
        [Authorize(Roles ="SUPER_USER,NORMAL_USER")]
        [HttpPut]
        public async Task<IActionResult> UpdateBookBorrowingRequest([FromBody] BookBorrowingRequestDtoUpdateStatus updateStatus)
        {
            await _bookBorrowingRequestService.UpdateBookBorrowingRequestStatusAsync(updateStatus);
            return Ok(ApiResponse<string>.Success("Book borrowing request updated successfully."));
        }
        [Authorize("SUPER_USER")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookBorrowedRequest(int pageSize = 10, int pageIndex = 1, int status = -1)
        {
            var bookBorrowingRequests = await _bookBorrowingRequestService.GetAllBookBorrowingRequestsAsync(status, pageIndex, pageSize);
            return Ok(ApiResponse<PaginatedList<BookBorrowingRequestResponse>>.Success(bookBorrowingRequests));
        }
        [Authorize("SUPER_USER")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookBorrowingRequest(int id)
        {
            await _bookBorrowingRequestService.DeleteBookBorrowingRequestAsync(id);
            return Ok(ApiResponse<string>.Success("Book borrowing request deleted successfully."));
        }
        [Authorize(Roles ="SUPER_USER,NORMAL_USER")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookBorrowingRequestById(int id)
        {
            var bookBorrowingRequest = await _bookBorrowingRequestService.GetBookBorrowingRequestByIdAsync(id);
            return Ok(ApiResponse<BookBorrowingRequestResponse>.Success(bookBorrowingRequest));
        }
        [Authorize(Roles = "SUPER_USER,NORMAL_USER")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookBorrowingRequestsByUserId(Guid userId, int pageSize = 10, int pageIndex = 1, int status = -1)
        {
            var bookBorrowingRequests = await _bookBorrowingRequestService.GetBookBorrowingRequestsByUserIdAsync(userId, pageIndex, pageSize, status);
            return Ok(ApiResponse<PaginatedList<BookBorrowingRequestResponse>>.Success(bookBorrowingRequests));
        }
        [Authorize(Roles = "SUPER_USER,NORMAL_USER")]
        [HttpGet("current-monthly-requests/{userId}")]
        public async Task<IActionResult> GetCurrentMonthlyRequetsCount(Guid userId)
        {
            var count = await _bookBorrowingRequestService.GetCurrentMonthlyRequetsCountAsync(userId);
            return Ok(ApiResponse<string>.Success(count.ToString(), "Current monthly requests count retrieved successfully."));
        }
    }
}