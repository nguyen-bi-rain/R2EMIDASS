using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using LMS.DTOs.Shared;
using LMS.Exceptions;

namespace LMS.Middlewares
{
    internal class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            
            var response = ApiResponse<string>.Failure(ex.Message);
            switch (ex)
            {
                case ResourceNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Resource Not Found";
                    response.Data = ex.Message;
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized Access";
                    response.Data = ex.Message;
                    break;
                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid Operation";
                    response.Data = ex.Message;
                    break;
                case DatabaseBadRequestException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Error when interacting with the database";
                    response.Data = ex.Message;
                    break;
                case ValidationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Validation Error";
                    response.Data = ex.Message;
                    break;
                case BadHttpRequestException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Bad Request";
                    response.Data = ex.Message;
                    break;
                case ResourceUniqueException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "Resource Conflict";
                    response.Data = ex.Message;
                    break;
                case DatabaseConflictException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = "Database Conflict";
                    response.Data = ex.Message;
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "Internal Server Error";
                    response.Data = "An unexpected error occurred.";
                    break;
}
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });
        }
    }
}