using ReservAr.Dtos.Sectors;

namespace ReservAr.Services.Interfaces
{
    public interface ISectorService
    {
        Task<SectorResponse> CreateAsync(CreateSectorRequest request);
        Task<SectorResponse?> UpdatePriceAsync(int sectorId, UpdateSectorRequest request);
        Task<SectorResponse?> GetByIdAsync(int sectorId);
        Task<List<SectorResponse>> SearchAsync(int? eventId, string? name);
    }
}