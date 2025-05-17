using EventBookingAPI.Data;
using EventBookingAPI.Helper.Exceptions;
using EventBookingAPI.Models;
using EventBookingAPI.Models.DTOs;
using EventBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBookingAPI.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;

        public EventService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventResponse>> GetAllEventsAsync(string userId = null, string category = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => e.Category == category);
            }

            var events = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Category = e.Category,
                Date = e.Date,
                Venue = e.Venue,
                Price = e.Price,
                ImageUrl = e.ImageUrl,
                IsBooked = userId != null && _context.Bookings
                    .Any(b => b.EventId == e.Id && b.UserId == userId)
            }).ToList();
        }

        public async Task<Event> CreateEventAsync(CreateEventRequest request)
        {
            string imageUrl = await HandleImageUpload(request.Image);

            var newEvent = new Event
            {
                Name = request.Name,
                Description = request.Description,
                Category = request.Category,
                Date = request.Date,
                Venue = request.Venue,
                Price = request.Price,
                ImageUrl = imageUrl
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return newEvent;
        }


        //public async Task<Event> CreateEventAsync(CreateEventRequest request)
        //{
        //    string imageUrl = null;
        //    if (request.Image != null)
        //    {
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        //        Directory.CreateDirectory(uploadsFolder); // في حال لم يكن المجلد موجود

        //        var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
        //        var filePath = Path.Combine(uploadsFolder, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await request.Image.CopyToAsync(stream);
        //        }

        //        imageUrl = $"/images/{fileName}";
        //    }

        //    var newEvent = new Event
        //    {
        //        Name = request.Name,
        //        Description = request.Description,
        //        Category = request.Category,
        //        Date = request.Date,
        //        Venue = request.Venue,
        //        Price = request.Price,
        //        ImageUrl = imageUrl
        //    };

        //    _context.Events.Add(newEvent);
        //    await _context.SaveChangesAsync();
        //    return newEvent;
        //}


        //public async Task<List<EventResponse>> GetAllEventsAsync(string userId = null)
        //{
        //    var events = await _context.Events.ToListAsync();

        //    return events.Select(e => new EventResponse
        //    {
        //        Id = e.Id,
        //        Name = e.Name,
        //        Description = e.Description,
        //        Category = e.Category,
        //        Date = e.Date,
        //        Venue = e.Venue,
        //        Price = e.Price,
        //        Image = e.ImageUrl,
        //        IsBooked = userId != null && _context.Bookings
        //            .Any(b => b.EventId == e.Id && b.UserId == userId)
        //    }).ToList();
        //}

        public async Task<EventResponse> GetEventByIdAsync(int id, string userId = null)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return null;

            var isBooked = false;
            if (userId != null)
            {
                isBooked = await _context.Bookings
                    .AnyAsync(b => b.EventId == @event.Id && b.UserId == userId);
            }

            return new EventResponse
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                Category = @event.Category,
                Date = @event.Date,
                Venue = @event.Venue,
                Price = @event.Price,
                ImageUrl = @event.ImageUrl,
                IsBooked = isBooked
            };
        }


        //public async Task<Event> CreateEventAsync(CreateEventRequest request)
        //{
        //    string imageUrl = null;
        //    if (request.Image != null)
        //    {
        //        var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
        //        var filePath = Path.Combine("wwwroot/images", fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await request.Image.CopyToAsync(stream);
        //        }

        //        imageUrl = $"/images/{fileName}";
        //    }
        //    var newEvent = new Event
        //    {
        //        Name = request.Name,
        //        Description = request.Description,
        //        Category = request.Category,
        //        Date = request.Date,
        //        Venue = request.Venue,
        //        Price = request.Price,
        //        Image = request.Image
        //    };

        //    _context.Events.Add(newEvent);
        //    await _context.SaveChangesAsync();
        //    return newEvent;
        //}
        public async Task UpdateEventAsync(int id, CreateEventRequest request)
        {
            var existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null)
            {
                throw new ApiException("Event not found");
            }

            // تحديث الصورة فقط إذا تم تقديم صورة جديدة
            if (request.Image != null)
            {
                // حذف الصورة القديمة إذا كانت موجودة
                if (!string.IsNullOrEmpty(existingEvent.ImageUrl))
                {
                    DeleteOldImage(existingEvent.ImageUrl);
                }

                existingEvent.ImageUrl = await HandleImageUpload(request.Image);
            }

            existingEvent.Name = request.Name;
            existingEvent.Description = request.Description;
            existingEvent.Category = request.Category;
            existingEvent.Date = request.Date;
            existingEvent.Venue = request.Venue;
            existingEvent.Price = request.Price;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();
        }

        //public async Task UpdateEventAsync(int id, CreateEventRequest request)
        //{
        //    var existingEvent = await _context.Events.FindAsync(id);
        //    if (existingEvent == null)
        //    {
        //        throw new ApiException("Event not found");
        //    }

        //    existingEvent.Name = request.Name;
        //    existingEvent.Description = request.Description;
        //    existingEvent.Category = request.Category;
        //    existingEvent.Date = request.Date;
        //    existingEvent.Venue = request.Venue;
        //    existingEvent.Price = request.Price;
        //    existingEvent.Image = request.Image;

        //    _context.Events.Update(existingEvent);
        //    await _context.SaveChangesAsync();
        //}

        public async Task DeleteEventAsync(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null)
            {
                throw new ApiException("Event not found");
            }

            // Check if there are bookings for this event
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (hasBookings)
            {
                throw new ApiException("Cannot delete event with existing bookings");
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
        }

        private async Task<string> HandleImageUpload(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            // التحقق من نوع الملف
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ApiException("Invalid image file type. Only JPG, JPEG, PNG, and GIF are allowed.");
            }

            // التحقق من حجم الملف (5MB كحد أقصى)
            if (image.Length > 5 * 1024 * 1024)
            {
                throw new ApiException("Image file size cannot exceed 5MB.");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }

        private void DeleteOldImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }
    }
}