using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using EventManagementSystem.Models;
using System.Text;

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
            if (IsReservedAdminUsername(request.Username))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "This username is reserved for administrators"
                };
            }

            if (await UserExistsAsync(request.Username))
                return new AuthResponse { Success = false, Message = "Username already exists" };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username.Trim(),
                PasswordHash = passwordHash,
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate simple token
            var token = GenerateSimpleToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id,
                Token = token // Add token
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "Invalid username or password" };

            if (user.Role == "admin")
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Administrators must use the admin login portal"
                };
            }

            // Generate simple token
            var token = GenerateSimpleToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id,
                Token = token // Add token
            };
        }

        public async Task<AuthResponse> AdminLoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "Invalid username or password" };

            if (user.Role != "admin")
                return new AuthResponse
                {
                    Success = false,
                    Message = "Access denied. Admin privileges required."
                };

            // Generate simple token
            var token = GenerateSimpleToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Admin login successful",
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id,
                Token = token // Add token
            };
        }

        private string GenerateSimpleToken(User user)
        {
            // Create a simple token format: base64(userId:username:timestamp)
            var tokenData = $"{user.Id}:{user.Username}:{DateTime.UtcNow.Ticks}";
            var tokenBytes = Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        private bool IsReservedAdminUsername(string username)
        {
            var reservedUsernames = new List<string>
            {
                "admin", "administrator", "superadmin", "root",
                "sysadmin", "moderator", "staff", "support"
            };

            return reservedUsernames.Contains(username.ToLower().Trim());
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}