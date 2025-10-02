using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(10)]
        public string Role { get; set; } = "user"; // "admin" or "user"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}