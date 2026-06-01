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
        private readonly IAuditLogServices _auditLogService;

        public ReservationService(ReservArDbContext context, ILogger<ReservationService> logger, IAuditLogServices auditLogService)
        {
            _context = context;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request)
        {
            await _auditLogService.Log(request.UserId, "RESERVATION_CREATE_ATTEMPT", "Seat", request.SeatId.ToString(), $"Intento de reserva para asiento {request.SeatId}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userExists = await _context.Users.AnyAsync(user => user.Id == request.UserId);

                if (!userExists)
                {
                    throw new KeyNotFoundException("El usuario indicado no existe.");
                }

                var seat = await _context.Seats.FirstOrDefaultAsync(seat => seat.Id == request.SeatId);

                if (seat is null)
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

                seat.Status = "RESERVADO";
                seat.Version += 1;

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _auditLogService.Log(request.UserId, "RESERVATION_CREATE_SUCCESS", "Reservation", reservation.Id.ToString(), $"Reserva {reservation.Id} creada con éxito");

                return MapToResponse(reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                await _auditLogService.Log(request.UserId, "RESERVATION_CREATE_CONFLICT", "Seat", request.SeatId.ToString(), "Conflicto de concurrencia: el asiento ya fue tomado por otro usuario.");
                throw new InvalidOperationException("CONFLICT_409: El asiento ya no está disponible.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "[CODE-ERROR] - Error al crear la reserva.");
                throw;
            }
        }

        public async Task ProcessPaymentAsync(int userId, Guid reservationId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId);

                if (reservation is null) throw new KeyNotFoundException("Reserva no encontrada.");
                if (reservation.Status != ReservationStatus.Pendiente) throw new InvalidOperationException("Reserva no válida para pago.");

                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.Id == reservation.SeatId);
                if (seat is null) throw new KeyNotFoundException("Asiento no encontrado.");

                // Transacción estricta: Actualizar reserva y asiento
                reservation.Status = "PAGADO";
                seat.Status = "VENDIDO";
                seat.Version += 1;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _auditLogService.Log(userId, "PAYMENT_SUCCESS", "Reservation", reservationId.ToString(), "Pago procesado y asiento vendido.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _auditLogService.Log(userId, "PAYMENT_FAILED", "Reservation", reservationId.ToString(), $"Error en pago: {ex.Message}");
                throw;
            }
        }

        public async Task<ReservationResponse?> GetByIdAsync(Guid reservationId)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(reservation => reservation.Id == reservationId);

            if (reservation is null)
            {
                return null;
            }

            return MapToResponse(reservation);
        }

        public async Task<ReservationResponse?> UpdateAsync(Guid reservationId, UpdateReservationRequest request)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(reservation => reservation.Id == reservationId);

            if (reservation is null)
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

        public async Task<int> ExpirePendingReservationsAsync()
        {
            var now = DateTime.UtcNow;
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var expiredReservations = await _context.Reservations
                    .Where(reservation =>
                        reservation.Status == ReservationStatus.Pendiente &&
                        reservation.ExpiresAt <= now)
                    .ToListAsync();

                foreach (var reservation in expiredReservations)
                {
                    reservation.Status = ReservationStatus.Expirado;

                    var seat = await _context.Seats
                        .FirstOrDefaultAsync(seat => seat.Id == reservation.SeatId);

                    if (seat != null && seat.Status.ToUpper() == "RESERVADO")
                    {
                        seat.Status = "DISPONIBLE";
                        seat.Version += 1;
                    }
                    await _auditLogService.Log(-1, "RESERVATION_AUTO_EXPIRE", "Reservation", reservation.Id.ToString(), "Liberación automática por tiempo agotado.");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return expiredReservations.Count;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Conflicto de concurrencia al procesar expiraciones.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "[CODE-ERROR] - Error al expirar reservaciones.");
                throw;
            }
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