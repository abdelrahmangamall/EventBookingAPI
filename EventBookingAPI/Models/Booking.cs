using System.ComponentModel.DataAnnotations;

namespace EventBookingAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    }
}
