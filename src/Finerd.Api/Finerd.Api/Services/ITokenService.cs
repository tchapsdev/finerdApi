using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;

namespace Finerd.Api.Services
{
    public interface ITokenService
    {
        Task<Tuple<string, string>> GenerateTokensAsync(int userId);
        Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task<bool> RemoveRefreshTokenAsync(User user);
    }
}
