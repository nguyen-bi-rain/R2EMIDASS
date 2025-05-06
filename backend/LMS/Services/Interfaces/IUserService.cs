using LMS.DTOs;

namespace LMS.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task RegisterAsync(RegisterRequest registerRequest);
        Task<RefreshTokenReponse> RefreshTokenAsync(RefreshTokenRequest token);
    }
}