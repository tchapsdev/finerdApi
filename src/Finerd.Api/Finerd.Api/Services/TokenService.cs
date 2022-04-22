using Finerd.Api.Data;
using Finerd.Api.Helpers;
using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Finerd.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        public TokenService(ApplicationDbContext applicationDbContext, IConfiguration configuration, ILogger<TokenService> logger)
        {
            this._applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _logger = logger;
            _jwtSettings = _configuration.GetRequiredSection("JwtSettings").Get<JwtSettings>();
        }

        public async Task<Tuple<string, string>> GenerateTokensAsync(int userId)
        {
            var accessToken = await GenerateAccessToken(userId);
            var refreshToken = await GenerateRefreshToken();

            var userRecord = await _applicationDbContext.Users.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == userId);

            if (userRecord == null)
            {
                return null;
            }

            var salt = PasswordHelper.GetSecureSalt();

            var refreshTokenHashed = PasswordHelper.HashUsingPbkdf2(refreshToken, salt);

            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                await RemoveRefreshTokenAsync(userRecord);

            }
            userRecord.RefreshTokens?.Add(new RefreshToken
            {
                ExpiryDate = DateTime.Now.AddDays(60),
                Ts = DateTime.Now,
                UserId = userId,
                TokenHash = refreshTokenHashed,
                TokenSalt = Convert.ToBase64String(salt)

            });

            await _applicationDbContext.SaveChangesAsync();

            var token = new Tuple<string, string>(accessToken, refreshToken);
            return token;
        }

        public async Task<bool> RemoveRefreshTokenAsync(User user)
        {
            var userRecord = await _applicationDbContext.Users.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == user.Id);

            if (userRecord == null)
            {
                return false;
            }

            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                var currentRefreshToken = userRecord.RefreshTokens.First();

                _applicationDbContext.RefreshTokens.Remove(currentRefreshToken);
            }

            return false;
        }

        public async Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = await _applicationDbContext.RefreshTokens.FirstOrDefaultAsync(o => o.UserId == refreshTokenRequest.UserId);

            var response = new ValidateRefreshTokenResponse();
            if (refreshToken == null)
            {
                response.Success = false;
                response.Error = "Invalid session or user is already logged out";
                response.ErrorCode = "R02";
                return response;
            }

            var refreshTokenToValidateHash = PasswordHelper.HashUsingPbkdf2(refreshTokenRequest.RefreshToken, Convert.FromBase64String(refreshToken.TokenSalt));

            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                response.Success = false;
                response.Error = "Invalid refresh token";
                response.ErrorCode = "R03";
                return response;
            }

            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                response.Success = false;
                response.Error = "Refresh token has expired";
                response.ErrorCode = "R04";
                return response;
            }

            response.Success = true;
            response.UserId = refreshToken.UserId;

            return response;
        }


        async Task<string> GenerateAccessToken(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_jwtSettings.AccessTokenSecret);
            var userRecord = await _applicationDbContext.Users.FirstOrDefaultAsync(e => e.Id == userId) ?? new User();
            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                //new Claim(ClaimTypes.Email, userRecord.Email),
                //new Claim(ClaimTypes.Name, userRecord.FirstName),
                //new Claim(ClaimTypes.GivenName, userRecord.FirstName),
                //new Claim(ClaimTypes.Surname, userRecord.LastName),
                new Claim(ClaimTypes.Sid, userId.ToString())
            });
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            //var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(key))), SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = signingCredentials,
                IssuedAt = DateTime.UtcNow
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.Run(() => tokenHandler.WriteToken(securityToken));
        }
        public static async Task<string> GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            await Task.Run(() => randomNumberGenerator.GetBytes(secureRandomBytes));
            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }

    }
}
