using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using LMS.DTOs;
using LMS.Exceptions;
using LMS.Helpers;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Interfaces;

namespace LMS.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private IHashPasswordHelper _hashPasswordHelper;
        public UserService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper,IHashPasswordHelper hashPasswordHelper)
        {
            _hashPasswordHelper = hashPasswordHelper;
            _userRepository = userRepository ;
            _tokenService = tokenService ;
            _mapper = mapper ;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(id));

            var user = await _userRepository.GetByIdAsync(id) 
                       ?? throw new ResourceNotFoundException("User not found");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                throw new ArgumentNullException(nameof(loginRequest));

            var user = await _userRepository.GetUserByUserName(loginRequest.UserName)
                       ?? throw new ResourceNotFoundException("User not found");

            if (!_hashPasswordHelper.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new DatabaseConflictException("Invalid password");

            var accessToken = await _tokenService.GenerateTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new LoginResponse
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
            };
        }

        public async Task RegisterAsync(RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                throw new ArgumentNullException(nameof(registerRequest));

            if (await _userRepository.GetUserByUserName(registerRequest.UserName) != null)
                throw new ResourceUniqueException("User already exists");

            var user = _mapper.Map<User>(registerRequest);
            user.PasswordHash = HashPasswordHelper.HashPassword(registerRequest.Password);
            user.RoleId = 3; // Assuming 3 is the default role ID for new users
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.AddAsync(user);
            if(await _userRepository.SaveChangesAsync() < 0)
                throw new DatabaseBadRequestException("Failed to register user");
        }

        public async Task<RefreshTokenReponse> RefreshTokenAsync(RefreshTokenRequest token)
        {
            var claimsPrincipal = _tokenService.GetPrincipalFromExpiredToken(token.AccessToken);
            Guid.TryParse(claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sid), out var userId);
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null){
                throw new UnauthorizedAccessException("Invalid user");
            }
            if(user.RefreshToken == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow){
                throw new UnauthorizedAccessException("Invalid refresh token");
            }
            
            var newAccessToken = await _tokenService.GenerateTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return new RefreshTokenReponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
        
    }
}