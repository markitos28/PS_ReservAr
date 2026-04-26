using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Events
{
    public class UpdateEventRequest
    {
        public DateTime? EventDate { get; set; }

        [MinLength(2)]
        [MaxLength(320)]
        public string? Venue { get; set; }

        [MaxLength(120)]
        public string? Status { get; set; }
    }
}