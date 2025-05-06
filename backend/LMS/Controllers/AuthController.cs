using LMS.DTOs;
using LMS.DTOs.Shared;
using LMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest("Login request cannot be null.");

            try
            {
                var response = await _userService.LoginAsync(loginRequest);
                return Ok(ApiResponse<LoginResponse>.Success(response, "Login successful."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Failure(ex.Message));
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            await _userService.RegisterAsync(registerRequest);
            return Ok(ApiResponse<string>.Success("", "User registered successfully."));
        }
        
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest tokenRequest)
        {

            var response = await _userService.RefreshTokenAsync(tokenRequest);
            return Ok(ApiResponse<RefreshTokenReponse>.Success(response, "Token refreshed successfully."));

        }
    }
}