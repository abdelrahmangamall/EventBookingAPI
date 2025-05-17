using EventBookingAPI.Models;
using EventBookingAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventBookingAPI.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponse>> GetAllEventsAsync(string userId = null, string category = null, int page = 1, int pageSize = 10);
        Task<EventResponse> GetEventByIdAsync(int id, string userId = null);
        Task<Event> CreateEventAsync(CreateEventRequest request);
        Task UpdateEventAsync(int id, CreateEventRequest request);
        Task DeleteEventAsync(int id);
    }
}