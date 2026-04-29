namespace ReservAr.Dtos.Seats
{
    public class SeatResponse
    {
        public Guid Id { get; set; }
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
    }
}