using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs
{
    public class VerifyEmailRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
