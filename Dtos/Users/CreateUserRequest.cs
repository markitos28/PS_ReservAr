using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Users
{
    public class CreateUserRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(120)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(120)]
        public string Password { get; set; }
    }
}