using System.Threading.Tasks;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email, string password);
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId);
    }
}
