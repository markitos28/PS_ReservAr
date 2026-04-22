using Microsoft.EntityFrameworkCore;
using ReservAr.Data;
using ReservAr.Dtos.Sectors;
using ReservAr.Models;
using ReservAr.Services.Interfaces;

namespace ReservAr.Services
{
    public class SectorService : ISectorService
    {
        private readonly ReservArDbContext _context;
        private readonly ILogger<SectorService> _logger;

        public SectorService(ReservArDbContext context, ILogger<SectorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SectorResponse> CreateAsync(CreateSectorRequest request)
        {
            var normalizedName = request.Name.Trim();

            var eventExists = await _context.Events.AnyAsync(evt => evt.Id == request.EventId);
            if (!eventExists)
            {
                throw new KeyNotFoundException("El evento indicado no existe.");
            }

            var sectorExists = await _context.Sectors.AnyAsync(sec =>
                sec.EventId == request.EventId &&
                sec.Name.ToLower() == normalizedName.ToLower()
            );

            if (sectorExists)
            {
                throw new InvalidOperationException("Ya existe un sector con ese nombre para el evento indicado.");
            }

            var entity = new Sector
            {
                EventId = request.EventId,
                Name = normalizedName,
                Price = request.Price,
                Capacity = request.Capacity
            };

            try
            {
                _context.Sectors.Add(entity);
                await _context.SaveChangesAsync();

                return MapToResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al crear el sector.");
                throw;
            }
        }

        public async Task<SectorResponse?> UpdatePriceAsync(int sectorId, UpdateSectorRequest request)
        {
            var entity = await _context.Sectors.FirstOrDefaultAsync(sec => sec.Id == sectorId);

            if (entity == null)
            {
                return null;
            }

            try
            {
                entity.Price = request.Price;
                await _context.SaveChangesAsync();

                return MapToResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al modificar el precio del sector.");
                throw;
            }
        }

        public async Task<SectorResponse?> GetByIdAsync(int sectorId)
        {
            var entity = await _context.Sectors
                .AsNoTracking()
                .FirstOrDefaultAsync(sec => sec.Id == sectorId);

            if (entity == null)
            {
                return null;
            }

            return MapToResponse(entity);
        }

        public async Task<List<SectorResponse>> SearchAsync(int? eventId, string? name)
        {
            var query = _context.Sectors
                .AsNoTracking()
                .AsQueryable();

            if (eventId.HasValue)
            {
                query = query.Where(sec => sec.EventId == eventId.Value);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var normalizedName = name.Trim().ToLower();
                query = query.Where(sec => sec.Name.ToLower().Contains(normalizedName));
            }

            var results = await query
                .OrderBy(sec => sec.EventId)
                .ThenBy(sec => sec.Name)
                .ToListAsync();

            var response = new List<SectorResponse>();

            foreach (var idx_tk in results)
            {
                response.Add(MapToResponse(idx_tk));
            }

            return response;
        }

        private static SectorResponse MapToResponse(Sector entity)
        {
            return new SectorResponse
            {
                Id = entity.Id,
                EventId = entity.EventId,
                Name = entity.Name,
                Price = entity.Price,
                Capacity = entity.Capacity
            };
        }
    }
}