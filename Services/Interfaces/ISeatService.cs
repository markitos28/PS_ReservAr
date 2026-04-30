using ReservAr.Dtos.Seats;

namespace ReservAr.Services.Interfaces
{
    public interface ISeatService
    {
        Task<SeatResponse> CreateAsync(CreateSeatRequest request);

        Task<SeatResponse?> UpdateAsync(Guid seatId, UpdateSeatRequest request);

        Task<SeatResponse?> GetByIdAsync(Guid seatId);

        Task<List<SeatResponse>> SearchAsync(
            int? seatNumber,
            string? rowIdentifier,
            int? sectorId,
            string? status,
            int? version
        );
    }
}