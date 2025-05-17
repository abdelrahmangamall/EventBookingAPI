using EventBookingAPI.Data;
using EventBookingAPI.Helper.Exceptions;
using EventBookingAPI.Models;
using EventBookingAPI.Models.DTOs;
using EventBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace EventBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponse> BookEventAsync(string userId, int eventId)
        {
            // Check if event exists
            var @event = await _context.Events.FindAsync(eventId);
            if (@event == null)
            {
            throw new ApiException("Event not found");
        }

        // Check if already booked
        var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.UserId == userId && b.EventId == eventId);
                
            if (existingBooking != null)
            {
                throw new ApiException("You have already booked this event");
            }

            var booking = new Booking
            {
                UserId = userId,
                EventId = eventId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingResponse
            {
                Id = booking.Id,
                EventId = @event.Id,
                EventName = @event.Name,
                EventDate = @event.Date,
                Venue = @event.Venue,
                BookingDate = booking.BookingDate
    };
        }

        public async Task CancelBookingAsync(int bookingId, string userId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null)
            {
                throw new ApiException("Booking not found or you don't have permission to cancel it");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BookingResponse>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Event)
                .ToListAsync();

            return bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                EventId = b.Event.Id,
                EventName = b.Event.Name,
                EventDate = b.Event.Date,
                Venue = b.Event.Venue,
                BookingDate = b.BookingDate
            }).ToList();
        }
    }
}
