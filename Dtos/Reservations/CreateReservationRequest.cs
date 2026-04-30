namespace ReservAr.Dtos.Reservations
{
    public class CreateReservationRequest
    {
        public int UserId { get; set; }
        public Guid SeatId { get; set; }
    }
}