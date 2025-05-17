namespace EventBookingAPI.Models.DTOs
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public DateTime BookingDate { get; set; }
    }
}