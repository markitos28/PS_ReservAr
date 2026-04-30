using Microsoft.EntityFrameworkCore;
using ReservAr.Data;
using ReservAr.Dtos.Seats;
using ReservAr.Models;
using ReservAr.Services.Interfaces;

namespace ReservAr.Services
{
    public class SeatService : ISeatService
    {
        private readonly ReservArDbContext _context;
        private readonly ILogger<SeatService> _logger;

        public SeatService(ReservArDbContext context, ILogger<SeatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
        {
            var normalizedRowIdentifier = request.RowIdentifier.Trim().ToUpper();
            var normalizedStatus = request.Status.Trim();

            ValidateSeatStatus(normalizedStatus);

            var sectorExists = await _context.Sectors.AnyAsync(sector => sector.Id == request.SectorId);

            if (!sectorExists)
            {
                throw new KeyNotFoundException("El sector indicado no existe.");
            }

            var seatExists = await _context.Seats.AnyAsync(seat =>
                seat.SectorId == request.SectorId &&
                seat.RowIdentifier.ToUpper() == normalizedRowIdentifier &&
                seat.SeatNumber == request.SeatNumber
            );

            if (seatExists)
            {
                throw new InvalidOperationException("Ya existe un asiento con esa numeración para el sector y fila indicados.");
            }

            var entity = new Seat
            {
                Id = Guid.NewGuid(),
                SectorId = request.SectorId,
                RowIdentifier = normalizedRowIdentifier,
                SeatNumber = request.SeatNumber,
                Status = normalizedStatus,
                Version = 0
            };

            try
            {
                _context.Seats.Add(entity);
                await _context.SaveChangesAsync();

                return MapToResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al crear el asiento.");
                throw;
            }
        }

        public async Task<SeatResponse?> UpdateAsync(Guid seatId, UpdateSeatRequest request)
        {
            var entity = await _context.Seats.FirstOrDefaultAsync(seat => seat.Id == seatId);

            if (entity == null)
            {
                return null;
            }

            var normalizedStatus = request.Status.Trim();

            ValidateSeatStatus(normalizedStatus);

            try
            {
                entity.Status = normalizedStatus;

                if (request.Version.HasValue)
                {
                    entity.Version = request.Version.Value;
                }
                else
                {
                    entity.Version += 1;
                }

                await _context.SaveChangesAsync();

                return MapToResponse(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al modificar el asiento.");
                throw;
            }
        }

        public async Task<SeatResponse?> GetByIdAsync(Guid seatId)
        {
            var entity = await _context.Seats
                .AsNoTracking()
                .FirstOrDefaultAsync(seat => seat.Id == seatId);

            if (entity == null)
            {
                return null;
            }

            return MapToResponse(entity);
        }

        public async Task<List<SeatResponse>> SearchAsync(
            int? seatNumber,
            string? rowIdentifier,
            int? sectorId,
            string? status,
            int? version)
        {
            var query = _context.Seats
                .AsNoTracking()
                .AsQueryable();

            if (seatNumber.HasValue)
            {
                query = query.Where(seat => seat.SeatNumber == seatNumber.Value);
            }

            if (!string.IsNullOrWhiteSpace(rowIdentifier))
            {
                var normalizedRowIdentifier = rowIdentifier.Trim().ToUpper();
                query = query.Where(seat => seat.RowIdentifier.ToUpper() == normalizedRowIdentifier);
            }

            if (sectorId.HasValue)
            {
                query = query.Where(seat => seat.SectorId == sectorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToLower();
                query = query.Where(seat => seat.Status.ToLower() == normalizedStatus);
            }

            if (version.HasValue)
            {
                query = query.Where(seat => seat.Version == version.Value);
            }

            var results = await query
                .OrderBy(seat => seat.SectorId)
                .ThenBy(seat => seat.RowIdentifier)
                .ThenBy(seat => seat.SeatNumber)
                .ToListAsync();

            var response = new List<SeatResponse>();

            foreach (var idx_tk in results)
            {
                response.Add(MapToResponse(idx_tk));
            }

            return response;
        }

        private static void ValidateSeatStatus(string status)
        {
            var validStatuses = new[] { "Disponible", "Reservado", "Vendido", "DISPONIBLE", "RESERVADO", "VENDIDO" };

            if (!validStatuses.Contains(status))
            {
                throw new InvalidOperationException("Estado inválido. Valores permitidos: DISPONIBLE, RESERVADO, VENDIDO.");
            }
        }

        private static SeatResponse MapToResponse(Seat entity)
        {
            return new SeatResponse
            {
                Id = entity.Id,
                SectorId = entity.SectorId,
                RowIdentifier = entity.RowIdentifier,
                SeatNumber = entity.SeatNumber,
                Status = entity.Status,
                Version = entity.Version
            };
        }
    }
}