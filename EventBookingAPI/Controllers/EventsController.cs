//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using EventBookingAPI.Services.Interfaces;
//using EventBookingAPI.Helper.Exceptions;

//namespace EventBookingAPI.Controllers
//{
    
//    [ApiController]
//    [Route("api/[controller]")]
//    public class EventsController : ControllerBase
//    {
//        private readonly IEventService _eventService;
//        private readonly IBookingService _bookingService;

//        public EventsController(IEventService eventService, IBookingService bookingService)
//        {
//            _eventService = eventService;
//            _bookingService = bookingService;
//        }

//        [HttpGet("GetAllEvents")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetAllEvents()
//        {
//            var events = await _eventService.GetAllEventsAsync();
//            return Ok(events);
//        }

//        [HttpGet("GetEventById{id}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetEventById(int id)
//        {
//            var @event = await _eventService.GetEventByIdAsync(id);
//        if (@event == null)
//        {
//            return NotFound();
//        }
//        return Ok(@event);
//        }

//        [HttpPost("book/{eventId}")]
//        [Authorize(Roles = "User")]
//        public async Task<IActionResult> BookEvent(int eventId)
//        {
//            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId))
//            {
//                return Unauthorized();
//            }

//            try
//            {
//                var booking = await _bookingService.BookEventAsync(userId, eventId);
//                return Ok(new { Message = "Event booked successfully!", BookingId = booking.Id });
//            }
//            catch (ApiException ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//    }
//}
