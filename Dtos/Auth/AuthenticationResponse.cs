using System.ComponentModel.DataAnnotations;

namespace ReservAr.Dtos.Auth
{
    public class AuthenticationResponse
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
    }
}