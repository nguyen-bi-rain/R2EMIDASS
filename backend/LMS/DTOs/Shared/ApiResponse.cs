namespace LMS.DTOs.Shared;

public class ApiResponse<T> where T : class
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Success(T data, string message = "", bool success = true)
    {
        return new ApiResponse<T>
        {
            Message = message,
            IsSuccess = success,
            Data = data
        };
    }
    public static ApiResponse<T> Failure(string message = "",T data = null)
    {
        return new ApiResponse<T>
        {
            Message = message,
            IsSuccess = false,
            Data = data
        };
    }
}