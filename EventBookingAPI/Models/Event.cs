namespace EventBookingAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Event
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(100)]
        public string Venue { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

    }
}
