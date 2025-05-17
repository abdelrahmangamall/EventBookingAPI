using EventBookingAPI.Helper.Exceptions;
using EventBookingAPI.Models.DTOs;
using EventBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventBookingAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public BookingsController(IBookingService bookingService, IStringLocalizer<SharedResource> localizer)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserBookings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpPost("events/{eventId}")]
        public async Task<IActionResult> BookEvent(int eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var booking = await _bookingService.BookEventAsync(userId, eventId);
            return Ok(booking);
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _bookingService.CancelBookingAsync(bookingId, userId);
                return Ok(_localizer["BookingCancelledSuccessfully"].Value);
            }
            catch (ApiException ex)
            {
                return BadRequest(_localizer[ex.Message].Value);
            }
        }

    }
}