using Microsoft.EntityFrameworkCore;
using ReservAr.Data;
using ReservAr.Dtos.Events;
using ReservAr.Models;
using ReservAr.Services.Interfaces;

namespace ReservAr.Services
{
    public class EventService : IEventService
    {
        private readonly ReservArDbContext _context;
        private readonly ILogger<EventService> _logger;

        public EventService(ReservArDbContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EventResponse> CreateAsync(CreateEventRequest request)
        {
            var normalizedName = request.Name.Trim();
            var normalizedVenue = request.Venue.Trim();
            var normalizedStatus = request.Status.Trim().ToUpper();

            var validStatuses = new[] { "DISPONIBLE", "SOLD-OUT", "FINALIZADA" };

            if (!validStatuses.Contains(normalizedStatus))
            {
                throw new InvalidOperationException("Estado inválido. Valores permitidos: DISPONIBLE, SOLD-OUT, FINALIZADA.");
            }

            var existingEvent = await _context.Events.AnyAsync(evt =>
                evt.Name.ToLower() == normalizedName.ToLower() &&
                evt.EventDate == request.EventDate
            );

            if (existingEvent)
            {
                throw new InvalidOperationException("Ya existe un evento con el mismo nombre y la misma fecha/hora.");
            }

            var entity = new Event
            {
                Name = normalizedName,
                EventDate = request.EventDate,
                Venue = normalizedVenue,
                Status = normalizedStatus
            };

            try
            {
                _context.Events.Add(entity);
                await _context.SaveChangesAsync();

                return MapToResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al crear el evento.");
                throw;
            }
        }

        public async Task<EventResponse?> UpdateAsync(int eventId, UpdateEventRequest request)
        {
            var entity = await _context.Events.FirstOrDefaultAsync(evt => evt.Id == eventId);

            if (entity == null)
            {
                return null;
            }

            var newEventDate = request.EventDate ?? entity.EventDate;
            var newVenue = request.Venue?.Trim() ?? entity.Venue;
            var newStatus = request.Status?.Trim().ToUpper() ?? entity.Status;

            var validStatuses = new[] { "DISPONIBLE", "SOLD-OUT", "FINALIZADA" };

            if (!validStatuses.Contains(newStatus))
            {
                throw new InvalidOperationException("Estado inválido. Valores permitidos: DISPONIBLE, SOLD-OUT, FINALIZADA.");
            }

            var duplicatedEvent = await _context.Events.AnyAsync(evt =>
                evt.Id != eventId &&
                evt.Name.ToLower() == entity.Name.ToLower() &&
                evt.EventDate == newEventDate
            );

            if (duplicatedEvent)
            {
                throw new InvalidOperationException("La modificación genera un evento duplicado con el mismo nombre y fecha/hora.");
            }

            try
            {
                entity.EventDate = newEventDate;
                entity.Venue = newVenue;
                entity.Status = newStatus;

                await _context.SaveChangesAsync();

                return MapToResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al modificar el evento.");
                throw;
            }
        }

        public async Task<EventResponse?> GetByIdAsync(int eventId)
        {
            var entity = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(evt => evt.Id == eventId);

            if (entity == null)
            {
                return null;
            }

            return MapToResponse(entity);
        }

        public async Task<List<EventResponse>> SearchAsync(
            int? eventId,
            string? name,
            DateTime? eventDate,
            string? venue,
            string? status)
        {
            var query = _context.Events
                .AsNoTracking()
                .Where(evt => evt.EventDate.Date >= DateTime.Today)
                .AsQueryable();

            if (eventId.HasValue)
            {
                query = query.Where(evt => evt.Id == eventId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(evt => evt.Name.ToLower().Contains(normalizedName));
            }

            if (eventDate.HasValue)
            {
                if (eventDate.Value.Date < DateTime.Today)
                {
                    throw new InvalidOperationException("EventDate solo permite consultar desde hoy hacia adelante.");
                }

                var fromDate = eventDate.Value.Date;
                var toDate = fromDate.AddDays(1);

                query = query.Where(evt => evt.EventDate >= fromDate && evt.EventDate < toDate);
            }

            if (!string.IsNullOrWhiteSpace(venue))
            {
                var normalizedVenue = venue.Trim().ToLower();
                query = query.Where(evt => evt.Venue.ToLower().Contains(normalizedVenue));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToUpper();
                query = query.Where(evt => evt.Status.ToUpper() == normalizedStatus);
            }

            var results = await query
                .OrderBy(evt => evt.EventDate)
                .ToListAsync();

            var response = new List<EventResponse>();

            foreach (var idx_tk in results)
            {
                response.Add(MapToResponse(idx_tk));
            }

            return response;
        }

        private static EventResponse MapToResponse(Event entity)
        {
            return new EventResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                EventDate = entity.EventDate,
                Venue = entity.Venue,
                Status = entity.Status
            };
        }
    }
}