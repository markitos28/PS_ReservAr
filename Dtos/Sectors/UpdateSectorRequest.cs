using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Sectors
{
    public class UpdateSectorRequest
    {
        [Required]
        [Range(0.01, 999999999)]
        public decimal Price { get; set; }
    }
}