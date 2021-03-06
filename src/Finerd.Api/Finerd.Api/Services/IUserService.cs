using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;

namespace Finerd.Api.Services
{
    public interface IUserService: IGenericService<User>
    {
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<SignupResponse> SignupAsync(SignupRequest signupRequest);
        Task<LogoutResponse> LogoutAsync(int userId);
        Task<SignupResponse> ProfileAsync(UserDto signupRequest);
        Task<User> GetByEmailAsync(string email);
    }
}
