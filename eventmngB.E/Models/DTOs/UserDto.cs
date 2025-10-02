using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(10)]
        public string Role { get; set; } = "user";
    }

    public class UpdateUserDto
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [StringLength(255, MinimumLength = 6)]
        public string? Password { get; set; }

        [StringLength(10)]
        public string Role { get; set; } = "user";
    }
}