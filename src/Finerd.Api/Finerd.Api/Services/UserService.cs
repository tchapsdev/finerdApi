using Finerd.Api.Data;
using Finerd.Api.Helpers;
using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;
using Microsoft.EntityFrameworkCore;

namespace Finerd.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext transactionsDbContext;
        private readonly ITokenService tokenService;

        public UserService(ApplicationDbContext transactionsDbContext, ITokenService tokenService)
        {
            this.transactionsDbContext = transactionsDbContext;
            this.tokenService = tokenService;
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = transactionsDbContext.Users.SingleOrDefault(user => user.Active && user.Email == loginRequest.Email);

            if (user == null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Email not found",
                    ErrorCode = "L02"
                };
            }
            var passwordHash = PasswordHelper.HashUsingPbkdf2(loginRequest.Password, Convert.FromBase64String(user.PasswordSalt));

            if (user.Password != passwordHash)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Invalid Password",
                    ErrorCode = "L03"
                };
            }

            var token = await System.Threading.Tasks.Task.Run(() => tokenService.GenerateTokensAsync(user.Id));

            return new TokenResponse
            {
                Success = true,
                AccessToken = token.Item1,
                RefreshToken = token.Item2
            };
        }

        public async Task<LogoutResponse> LogoutAsync(int userId)
        {
            var refreshToken = await transactionsDbContext.RefreshTokens.FirstOrDefaultAsync(o => o.UserId == userId);

            if (refreshToken == null)
            {
                return new LogoutResponse { Success = true };
            }

            transactionsDbContext.RefreshTokens.Remove(refreshToken);

            var saveResponse = await transactionsDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new LogoutResponse { Success = true };
            }

            return new LogoutResponse { Success = false, Error = "Unable to logout user", ErrorCode = "L04" };

        }

        public async Task<SignupResponse> SignupAsync(SignupRequest signupRequest)
        {
            var existingUser = await transactionsDbContext.Users.SingleOrDefaultAsync(user => user.Email == signupRequest.Email);

            if (existingUser != null)
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "User already exists with the same email",
                    ErrorCode = "S02"
                };
            }

            if (signupRequest.Password != signupRequest.ConfirmPassword)
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "Password and confirm password do not match",
                    ErrorCode = "S03"
                };
            }

            if (signupRequest.Password.Length < 6) // This can be more complicated than only length, you can check on alphanumeric and or special characters
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "Password is weak",
                    ErrorCode = "S04"
                };
            }

            var salt = PasswordHelper.GetSecureSalt();
            var passwordHash = PasswordHelper.HashUsingPbkdf2(signupRequest.Password, salt);

            var user = new User
            {
                Email = signupRequest.Email,
                Password = passwordHash,
                PasswordSalt = Convert.ToBase64String(salt),
                FirstName = signupRequest.FirstName,
                LastName = signupRequest.LastName,
                CreatedAt = DateTime.UtcNow,
                Active = true // You can save is false and send confirmation email to the user, then once the user confirms the email you can make it true
            };

            await transactionsDbContext.Users.AddAsync(user);

            var saveResponse = await transactionsDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new SignupResponse { Success = true, Email = user.Email , Id = user.Id};
            }

            return new SignupResponse
            {
                Success = false,
                Error = "Unable to save the user",
                ErrorCode = "S05"
            };

        }


        public async Task<SignupResponse> ProfileAsync(UserDto signupRequest)
        {
            var email = signupRequest.Email?.ToUpper() ?? "";
            var existingUser = await transactionsDbContext.Users.SingleOrDefaultAsync(user => user.Email.ToUpper() == email || user.Id == signupRequest.Id);
            if (existingUser == null)
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "User dos not exist with the same email",
                    ErrorCode = "S02"
                };
            }
            existingUser.FirstName = signupRequest.FirstName;
            existingUser.LastName = signupRequest.LastName;
            transactionsDbContext.Users.Update(existingUser);
            var saveResponse = await transactionsDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new SignupResponse { Success = true, Email = existingUser.Email , Id = existingUser.Id};
            }

            return new SignupResponse
            {
                Success = false,
                Error = "Unable to save the user profile",
                ErrorCode = "S05"
            };

        }
    }
}
