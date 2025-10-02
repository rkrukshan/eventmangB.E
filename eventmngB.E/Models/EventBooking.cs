using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EventManagementSystem.Models
{
    public class EventBooking
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int EventId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Event Event { get; set; } = null!;

        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}