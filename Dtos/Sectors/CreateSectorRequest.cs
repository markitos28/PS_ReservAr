using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Sectors
{
    public class CreateSectorRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(120)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }
}