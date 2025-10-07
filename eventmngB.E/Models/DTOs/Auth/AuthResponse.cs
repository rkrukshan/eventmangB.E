namespace EventManagementSystem.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Role { get; set; }
        public int? UserId { get; set; }
        public string? Token { get; set; } // Add this line
    }
}