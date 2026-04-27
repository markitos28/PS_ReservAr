namespace ReservAr.Dtos.Users
{
    /// <summary>
    /// Registro de usuario DTO
    /// </summary>
    public class UserRegisterDTO
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}