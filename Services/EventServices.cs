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

        private static readonly string[] ValidStatuses = { "DISPONIBLE", "SOLD-OUT", "FINALIZADA" };

        public EventService(ReservArDbContext context, ILogger<EventService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EventResponse> CreateAsync(CreateEventRequest request)
        {
            var normalizedName = NormalizeString(request.Name);
            var normalizedVenue = NormalizeString(request.Venue);
            var normalizedStatus = NormalizeString(request.Status).ToUpper();

            ValidateStatus(normalizedStatus);

            var normalizedEventDate = DateTime.SpecifyKind(request.EventDate, DateTimeKind.Utc);

            if (await IsEventDuplicateAsync(normalizedName, normalizedEventDate))
            {
                throw new InvalidOperationException("Ya existe un evento con el mismo nombre y la misma fecha/hora.");
            }

            var entity = new Event
            {
                Name = normalizedName,
                EventDate = normalizedEventDate,
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

            if (entity is null)
            {
                return null;
            }

            var newEventDate = request.EventDate.HasValue
                ? DateTime.SpecifyKind(request.EventDate.Value, DateTimeKind.Utc)
                : entity.EventDate;

            var newVenue = NormalizeString(request.Venue) ?? entity.Venue;
            var newStatus = NormalizeString(request.Status)?.ToUpper() ?? entity.Status;

            ValidateStatus(newStatus);

            if (await IsEventDuplicateAsync(entity.Name, newEventDate, eventId))
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

            if (entity is null)
            {
                return null;
            }

            return MapToResponse(entity);
        }

        public async Task<PagedResponse<EventResponse>> SearchAsync(
            int? eventId,
            string? name,
            DateTime? eventDate,
            string? venue,
            string? status,
            int pageNumber,
            int pageSize)
        {
            var today = DateTime.UtcNow.Date;

            var query = _context.Events
                .AsNoTracking()
                .Where(evt => evt.EventDate >= today)
                .AsQueryable();

            ApplyFilters(ref query, eventId, name, eventDate, venue, status, today);

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(evt => evt.EventDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var responseItems = items.Select(MapToResponse).ToList();

            var response = new PagedResponse<EventResponse>(responseItems, totalRecords, pageNumber, pageSize);

            return response;
        }

        private static void ValidateStatus(string status)
        {
            if (!ValidStatuses.Contains(status))
            {
                throw new InvalidOperationException("Estado inválido. Valores permitidos: DISPONIBLE, SOLD-OUT, FINALIZADA.");
            }
        }

        private static string NormalizeString(string? value)
        {
            return value?.Trim() ?? "";
        }

        private async Task<bool> IsEventDuplicateAsync(string name, DateTime eventDate, int? excludeId = null)
        {
            var query = _context.Events.Where(evt =>
                evt.Name.ToLower() == name.ToLower() &&
                evt.EventDate == eventDate);

            if (excludeId.HasValue)
            {
                query = query.Where(evt => evt.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        private void ApplyFilters(ref IQueryable<Event> query, int? eventId, string? name, DateTime? eventDate, string? venue, string? status, DateTime today)
        {
            if (eventId.HasValue)
            {
                query = query.Where(evt => evt.Id == eventId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = NormalizeString(name).ToLower();
                query = query.Where(evt => evt.Name.ToLower().Contains(normalizedName));
            }

            if (eventDate.HasValue)
            {
                var requestedDate = DateTime.SpecifyKind(eventDate.Value.Date, DateTimeKind.Utc);

                if (requestedDate < today)
                {
                    throw new InvalidOperationException("EventDate solo permite consultar desde hoy hacia adelante.");
                }

                var fromDate = requestedDate;
                var toDate = fromDate.AddDays(1);

                query = query.Where(evt => evt.EventDate >= fromDate && evt.EventDate < toDate);
            }

            if (!string.IsNullOrWhiteSpace(venue))
            {
                var normalizedVenue = NormalizeString(venue).ToLower();
                query = query.Where(evt => evt.Venue.ToLower().Contains(normalizedVenue));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = NormalizeString(status).ToUpper();
                query = query.Where(evt => evt.Status.ToUpper() == normalizedStatus);
            }
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