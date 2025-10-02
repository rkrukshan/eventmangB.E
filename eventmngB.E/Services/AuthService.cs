using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;

namespace EventManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if user already exists
            if (await UserExistsAsync(request.Username))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Username and password are required"
                };
            }

            if (request.Password.Length < 6)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Password must be at least 6 characters long"
                };
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user
            var user = new User
            {
                Username = request.Username.Trim(),
                PasswordHash = passwordHash,
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id
            };
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }
    }
}