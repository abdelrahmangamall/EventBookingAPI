using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EventBookingAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
