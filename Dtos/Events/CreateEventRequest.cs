using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Events
{
    public class CreateEventRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(320)]
        public string Venue { get; set; }

        [Required]
        [MaxLength(120)]
        public string Status { get; set; }
    }
}