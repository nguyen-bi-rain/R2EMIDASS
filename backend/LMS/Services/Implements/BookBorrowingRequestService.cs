using AutoMapper;
using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Exceptions;
using LMS.Models.Enitties;
using LMS.Models.Enums;
using LMS.Repositories.Interfaces;
using LMS.Services.Interfaces;
using LMS.Specifications;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services.Implements
{
    public class BookBorrowingRequestService : IBookBorrowingRequestService
    {
        private readonly IBookBorrowingRequestRepository _bookBorrowingRequestRepository;
        private readonly IBookBorrowingRequestDetailsRepository _bookBorrowingRequestDetailsRepository;
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly ISendEmailSerivce _sendEmailSerivce;

        public BookBorrowingRequestService(
            ISendEmailSerivce sendEmailSerivce,
            IUserService userService,
            IBookBorrowingRequestRepository bookBorrowingRequestRepository,
            IBookService bookService,
            IMapper mapper,
            IBookBorrowingRequestDetailsRepository bookBorrowingRequestDetailsRepository)
        {
            _sendEmailSerivce = sendEmailSerivce;
            _userService = userService;
            _bookBorrowingRequestRepository = bookBorrowingRequestRepository;
            _bookService = bookService;
            _bookBorrowingRequestDetailsRepository = bookBorrowingRequestDetailsRepository;
            _mapper = mapper;
        }

        public async Task CreateBookBorrowingRequestAsync(BookBorrowingRequestDtoCreate bookBorrowingRequest)
        {
            await ValidateMonthlyRequestLimitAsync(bookBorrowingRequest.RequestorId);

            if (bookBorrowingRequest.BookBorrowingRequestDetails.Count > 5)
            {
                throw new InvalidOperationException("You can only borrow up to 5 books at a time.");
            }

            var bookBorrowingRequestEntity = _mapper.Map<BookBorrowingRequest>(bookBorrowingRequest);

            await _bookBorrowingRequestRepository.BeginTransactionAsync();

            try
            {
                bookBorrowingRequestEntity.DateExpired = DateTime.Now.AddDays(30);
                var createdRequest = await _bookBorrowingRequestRepository.AddAsync(bookBorrowingRequestEntity);
                await _bookBorrowingRequestRepository.SaveChangesAsync();

                foreach (var item in bookBorrowingRequest.BookBorrowingRequestDetails)
                {
                    var bookBorrowingRequestDetails = new BookBorrowingRequestDetailsDtoCreate
                    {
                        BookId = item.BookId,
                        BookBorrowingRequestId = createdRequest.Id,
                        CreatedAt = DateTime.Now
                    };

                    var bookBorrowingRequestDetailsEntity = _mapper.Map<BookBorrowingRequestDetails>(bookBorrowingRequestDetails);
                    await _bookService.UpdateBookQuantityAsync(item.BookId, -1);
                    await _bookBorrowingRequestDetailsRepository.AddAsync(bookBorrowingRequestDetailsEntity);
                }
                await _bookBorrowingRequestRepository.CommitTransactionAsync();
            }
            catch
            {
                await _bookBorrowingRequestRepository.RollbackTransactionAsync();
                throw;
            }
        }

        private async Task ValidateMonthlyRequestLimitAsync(Guid requestorId)
        {
            var currentMonthRequests = await _bookBorrowingRequestRepository.GetQuery()
                .Where(x => x.RequestorId == requestorId &&
                            x.DateRequest.Month == DateTime.Now.Month &&
                            x.DateRequest.Year == DateTime.Now.Year)
                .CountAsync();

            if (currentMonthRequests >= 3)
            {
                throw new InvalidOperationException("You can only borrow up to 3 requests per month.");
            }
        }
        public async Task DeleteBookBorrowingRequestAsync(int id)
        {
            var request = await _bookBorrowingRequestRepository.GetByIdAsync(id)
                ?? throw new ResourceNotFoundException("Book borrowing request not found.");

            await _bookBorrowingRequestRepository.DeleteAsync(request);

            if (await _bookBorrowingRequestRepository.SaveChangesAsync() < 0)
            {
                throw new InvalidOperationException("Failed to delete book borrowing request.");
            }
        }

        public async Task<PaginatedList<BookBorrowingRequestResponse>> GetAllBookBorrowingRequestsAsync(int status, int pageIndex = 1, int pageSize = 10)
        {
            var spec = new BookBorrowingRequestPagingSpecification(pageIndex, pageSize, status, null);
            var result = await _bookBorrowingRequestRepository.GetPagedBookBorrowingRequestsAsync(spec);

            if (result.query == null || !result.query.Any())
            {
                throw new ResourceNotFoundException("No book borrowing requests found.");
            }
            var requests = result.query;
            var model = new List<BookBorrowingRequestResponse>();

            foreach (var request in requests)
            {
                var userName = (await _userService.GetUserByIdAsync(request.RequestorId)).UserName;
                var approverName = request.ApproverId.HasValue
                    ? (await _userService.GetUserByIdAsync(request.ApproverId.Value)).UserName
                    : null;

                model.Add(new BookBorrowingRequestResponse
                {
                    Id = request.Id,
                    RequestName = userName,
                    ApproveName = approverName,
                    DateRequest = request.DateRequest,
                    Status = request.Status,
                    DateExpired = request.DateExpired,
                    BookBorrowingRequestDetails = _mapper.Map<IEnumerable<BookBorrowingRequestDetailsDto>>(request.BookBorrowingRequestDetails)
                });
            }

            return new PaginatedList<BookBorrowingRequestResponse>(model.ToList(), result.totalCount, pageIndex, pageSize);
        }

        public async Task<BookBorrowingRequestResponse> GetBookBorrowingRequestByIdAsync(int id)
        {
            var request = await _bookBorrowingRequestRepository.GetQuery()
                .Include(r => r.BookBorrowingRequestDetails)
                .ThenInclude(d => d.Book)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new KeyNotFoundException("Book borrowing request not found.");

            return _mapper.Map<BookBorrowingRequestResponse>(request);
        }

        public async Task<PaginatedList<BookBorrowingRequestResponse>> GetBookBorrowingRequestsByUserIdAsync(Guid userId, int pageIndex = 1, int pageSize = 10, int status = -1)
        {
            var spec = new BookBorrowingRequestPagingSpecification(pageIndex, pageSize, status, userId);
            var result = await _bookBorrowingRequestRepository.GetPagedBookBorrowingRequestsAsync(spec);

            if (result.query == null || !result.query.Any())
            {
                throw new ResourceNotFoundException("No book borrowing requests found.");
            }
            var requests = result.query;
            var model = new List<BookBorrowingRequestResponse>();

            foreach (var request in requests)
            {
                var userName = (await _userService.GetUserByIdAsync(request.RequestorId)).UserName;
                var approverName = request.ApproverId.HasValue 
                    ? (await _userService.GetUserByIdAsync(request.ApproverId.Value)).UserName
                    : null;

                model.Add(new BookBorrowingRequestResponse
                {
                    Id = request.Id,
                    RequestName = userName,
                    ApproveName = approverName,
                    DateRequest = request.DateRequest,
                    Status = request.Status,
                    DateExpired = request.DateExpired,
                    BookBorrowingRequestDetails = _mapper.Map<IEnumerable<BookBorrowingRequestDetailsDto>>(request.BookBorrowingRequestDetails)
                });
            }

            return new PaginatedList<BookBorrowingRequestResponse>(model.ToList(), result.totalCount, pageIndex, pageSize);
        }

        public async Task UpdateBookBorrowingRequestAsync(BookBorrowingRequestDtoUpdate bookBorrowingRequest)
        {
            var existingRequest = await _bookBorrowingRequestRepository.GetByIdAsync(bookBorrowingRequest.Id)
                ?? throw new KeyNotFoundException("Book borrowing request not found.");

            _mapper.Map(bookBorrowingRequest, existingRequest);

            await _bookBorrowingRequestRepository.UpdateAsync(existingRequest);

            if (await _bookBorrowingRequestRepository.SaveChangesAsync() < 0)
            {
                throw new InvalidOperationException("Failed to update book borrowing request.");
            }
        }

        public async Task UpdateBookBorrowingRequestStatusAsync(BookBorrowingRequestDtoUpdateStatus bookBorrowingRequestStatus)
        {
            var request = await _bookBorrowingRequestRepository.GetQuery()
                .Include(r => r.BookBorrowingRequestDetails)
                .FirstOrDefaultAsync(r => r.Id == bookBorrowingRequestStatus.Id)
                ?? throw new ResourceNotFoundException("Book borrowing request not found.");

            request.Status = bookBorrowingRequestStatus.Status;

            request.ApproverId = bookBorrowingRequestStatus.ApproverId;
            request.UpdatedAt = DateTime.Now;
            await _bookBorrowingRequestRepository.UpdateAsync(request);

            if (bookBorrowingRequestStatus.Status == Status.Rejected)
            {
                foreach (var item in request.BookBorrowingRequestDetails)
                {
                    await _bookService.UpdateBookQuantityAsync(item.BookId, 1);
                }
            }

            if (await _bookBorrowingRequestRepository.SaveChangesAsync() < 0)
            {
                throw new DatabaseBadRequestException("Failed to update book borrowing request status.");
            }
            var user = await _userService.GetUserByIdAsync(request.RequestorId);
            var approver = await _userService.GetUserByIdAsync(request.ApproverId.Value);
            var emailContent = $"Dear {user.UserName}, your book borrowing request has been {request.Status.ToString().ToLower()} by {approver.UserName}.";
            var emailSubject = $"Book Borrowing Request is {request.Status}";
            var EmailBody = new EmailBodyModel {
                DateRequest = request.DateRequest,
                Id = request.Id,
                Status = request.Status.ToString(),
                Message = emailContent,
                UserName = user.UserName,
                StatusColor = request.Status == Status.Approved ? "green" : "red"
            };
            await _sendEmailSerivce.SendEmalWithBodyAsync(user.Email,emailSubject,"", EmailBody);
        }

        public async Task<int> GetCurrentMonthlyRequetsCountAsync(Guid userId)
        {
            var result = await _bookBorrowingRequestRepository.GetQuery()
                .Where(x => x.DateRequest.Month == DateTime.Now.Month && x.DateRequest.Year == DateTime.Now.Year && x.RequestorId == userId)
                .CountAsync();
            return result;
        }
    }
}
