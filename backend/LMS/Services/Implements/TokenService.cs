using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LMS.Models.Enitties;
using LMS.Repositories.Interfaces;
using LMS.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly string _jwt;
        private readonly TimeSpan _tokenLifetime;
        private readonly IUserRepository _userRepository;

        public TokenService(IUserRepository userRepository)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWTSETTINGS__KEY");
            _jwt = jwtKey;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            _issuer = Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER");
            _audience = Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE");
            _tokenLifetime = TimeSpan.FromMinutes(Convert.ToDouble(Environment.GetEnvironmentVariable("JWTSETTINGS__EXPIRETIME")));
            _userRepository = userRepository;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsAsync(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
            };

            var role = await _userRepository.GetUserRoleAsync(user.Id);
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(_tokenLifetime),
                signingCredentials: signingCredentials
            );
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new SecurityTokenException(nameof(token));

            if (token.Split('.').Length != 3)
                throw new SecurityTokenException("Invalid token format. JWT must have three segments.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwt);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}