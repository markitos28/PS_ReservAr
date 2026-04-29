using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Seats
{
    public class UpdateSeatRequest
    {
        [Required]
        [MaxLength(60)]
        public string Status { get; set; }

        public int? Version { get; set; }
    }
}