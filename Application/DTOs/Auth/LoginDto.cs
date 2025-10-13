using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email es requerido")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password es requerido")]
        public string Password { get; set; } = string.Empty;
    }
}
