using System.ComponentModel.DataAnnotations;

namespace EventBookingAPI.Models.DTOs
{
    public class EventResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsBooked { get; set; } // For authenticated users
    }
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
    public class CreateEventRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Venue { get; set; }
        [Required]
        public decimal Price { get; set; }
        public IFormFile Image { get; set; }
    }
}