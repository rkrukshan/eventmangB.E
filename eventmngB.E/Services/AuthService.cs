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
            if (await UserExistsAsync(request.Username))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

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

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully",
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id
            };
        }

        //public async Task<AuthResponse> LoginAsync(LoginRequest request)
        //{
        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Username == request.Username);

        //    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        //    {
        //        return new AuthResponse
        //        {
        //            Success = false,
        //            Message = "Invalid username or password"
        //        };
        //    }

        //    return new AuthResponse
        //    {
        //        Success = true,
        //        Message = "Login successful",
        //        Username = user.Username,
        //        Role = user.Role,
        //        UserId = user.Id
        //    };
        //}
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not found" };

            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordValid)
                return new AuthResponse { Success = false, Message = "Invalid password" };

            if (user.Role != "admin")
                return new AuthResponse { Success = false, Message = "Not an admin user" };

            // generate JWT token, etc.
            return new AuthResponse
            {
                Success = true,
                //Token = "<jwt-token-here>",
                //User = new { user.Id, user.Username, user.Role }
            };
        }


        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }
    }
}