using EventBookingAPI.Helper.Exceptions;
using EventBookingAPI.Models.DTOs;
using EventBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace EventBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]

    public class AdminController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AdminController(IEventService eventService, IStringLocalizer<SharedResource> localizer)
        {
            _eventService = eventService;
            _localizer = localizer;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);
            if (@event == null)
            {
            return NotFound();
        }
            return Ok(@event);
        }
      
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventRequest request)
        {
            try
            {
                var newEvent = await _eventService.CreateEventAsync(request);
                return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
            }
            catch (ApiException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPagedEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? category = null)
        {
            var events = await _eventService.GetAllEventsAsync(null, category, page, pageSize);
            return Ok(events);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventRequest request)
        {
            try
            {
                await _eventService.UpdateEventAsync(id, request);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return Ok(_localizer["EventDeletedSuccessfully"].Value);
            }
            catch (ApiException ex)
            {
                return BadRequest(_localizer[ex.Message].Value);
            }
        }
    }
}