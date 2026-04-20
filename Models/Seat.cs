namespace ReservAr.Models
{
    public class Seat
    {
        public Guid Id {get; set;}
        public int SectorId {get; set;}
        public required string RowIdentifier {get; set;}
        public int SeatNumber {get; set;}
        public string? Status {get; set;}
        public int Version {get; set;}

    }
}
