using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);

        Task<AuthResponse> AdminLoginAsync(LoginRequest request);
        Task<bool> UserExistsAsync(string username);
    }
}