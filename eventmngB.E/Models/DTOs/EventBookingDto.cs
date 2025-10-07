using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Models
{
    public class EventBookingDto
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
    }

    public class CreateEventBookingDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class UpdateEventBookingDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}