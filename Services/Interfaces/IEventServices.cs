using ReservAr.Dtos.Events;

namespace ReservAr.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventResponse> CreateAsync(CreateEventRequest request);
        Task<EventResponse?> UpdateAsync(int eventId, UpdateEventRequest request);
        Task<EventResponse?> GetByIdAsync(int eventId);
        Task<List<EventResponse>> SearchAsync(
            int? eventId,
            string? name,
            DateTime? eventDate,
            string? venue,
            string? status
        );
    }
}