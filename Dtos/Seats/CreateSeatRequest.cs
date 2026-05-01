using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Seats
{
    public class CreateSeatRequest
    {
        [Required]
        public int SectorId { get; set; }

        [Required]
        [MaxLength(60)]
        public string RowIdentifier { get; set; }

        [Required]
        public int SeatNumber { get; set; }

        [Required]
        [MaxLength(60)]
        public string Status { get; set; }
    }
}