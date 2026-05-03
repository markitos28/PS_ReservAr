using ReservAr.Dtos.Reservations;

namespace ReservAr.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationResponse> CreateAsync(CreateReservationRequest request);
        Task<ReservationResponse?> GetByIdAsync(Guid reservationId);

        Task<ReservationResponse?> UpdateAsync(Guid reservationId, UpdateReservationRequest request);

        Task<List<ReservationResponse>> SearchAsync(
            int? userId,
            Guid? seatId,
            string? status
        );

        Task<int> ExpirePendingReservationsAsync();
    }
}