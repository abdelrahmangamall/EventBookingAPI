using EventBookingAPI.Models.DTOs;

namespace EventBookingAPI.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> BookEventAsync(string userId, int eventId);
        Task CancelBookingAsync(int bookingId, string userId);
        Task<List<BookingResponse>> GetUserBookingsAsync(string userId);
    }
}