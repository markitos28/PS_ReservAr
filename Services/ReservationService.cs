using Microsoft.EntityFrameworkCore;
using ReservAr.Data;
using ReservAr.Dtos.Reservations;
using ReservAr.Models;
using ReservAr.Services.Interfaces;
using ReservAr.Helpers;

namespace ReservAr.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ReservArDbContext _context;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(ReservArDbContext context, ILogger<ReservationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request)
        {
            var userExists = await _context.Users.AnyAsync(user => user.Id == request.UserId);

            if (!userExists)
            {
                throw new KeyNotFoundException("El usuario indicado no existe.");
            }

            var seat = await _context.Seats.FirstOrDefaultAsync(seat => seat.Id == request.SeatId);

            if (seat == null)
            {
                throw new KeyNotFoundException("El asiento indicado no existe.");
            }

            if (seat.Status.ToUpper() != "DISPONIBLE")
            {
                throw new InvalidOperationException("El asiento no está disponible.");
            }

            var activeReservationExists = await _context.Reservations.AnyAsync(reservation =>
                reservation.SeatId == request.SeatId &&
                reservation.Status == ReservationStatus.Pendiente &&
                reservation.ExpiresAt > DateTime.UtcNow
            );

            if (activeReservationExists)
            {
                throw new InvalidOperationException("El asiento ya tiene una reserva pendiente activa.");
            }

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                SeatId = request.SeatId,
                Status = ReservationStatus.Pendiente,
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            try
            {
                seat.Status = "RESERVADO";
                seat.Version += 1;

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return MapToResponse(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al crear la reserva.");
                throw;
            }
        }

        public async Task<ReservationResponse?> GetByIdAsync(Guid reservationId)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(reservation => reservation.Id == reservationId);

            if (reservation == null)
            {
                return null;
            }

            return MapToResponse(reservation);
        }

        public async Task<ReservationResponse?> UpdateAsync(Guid reservationId, UpdateReservationRequest request)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(reservation => reservation.Id == reservationId);

            if (reservation == null)
            {
                return null;
            }

            var normalizedStatus = request.Status.Trim().ToUpper();

            ValidateReservationStatus(normalizedStatus);

            reservation.Status = normalizedStatus;

            await _context.SaveChangesAsync();

            return MapToResponse(reservation);
        }

        public async Task<List<ReservationResponse>> SearchAsync(
            int? userId,
            Guid? seatId,
            string? status)
        {
            var query = _context.Reservations
                .AsNoTracking()
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(reservation => reservation.UserId == userId.Value);
            }

            if (seatId.HasValue)
            {
                query = query.Where(reservation => reservation.SeatId == seatId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalizedStatus = status.Trim().ToUpper();
                query = query.Where(reservation => reservation.Status.ToUpper() == normalizedStatus);
            }

            var results = await query
                .OrderByDescending(reservation => reservation.ReservedAt)
                .ToListAsync();

            var response = new List<ReservationResponse>();

            foreach (var idx_tk in results)
            {
                response.Add(MapToResponse(idx_tk));
            }

            return response;
        }

        private static void ValidateReservationStatus(string status)
        {
            var validStatuses = new[]
            {
                ReservationStatus.Pendiente,
                ReservationStatus.Pagando,
                ReservationStatus.Expirado
            };

            if (!validStatuses.Contains(status))
            {
                throw new InvalidOperationException("Estado inválido. Valores permitidos: PENDIENTE, PAGANDO, EXPIRADO.");
            }
        }

        private static ReservationResponse MapToResponse(Reservation entity)
        {
            return new ReservationResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                SeatId = entity.SeatId,
                Status = entity.Status,
                ReservedAt = entity.ReservedAt,
                ExpiresAt = entity.ExpiresAt
            };
        }
    }
}