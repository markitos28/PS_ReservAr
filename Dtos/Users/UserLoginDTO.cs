namespace   ReservAr.Dtos.Users
{
    /// <summary>
    /// Login de usuario DTO
    /// </summary>
    public class UserLoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}