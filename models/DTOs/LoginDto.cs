using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models.DTOs
{
    [ExcludeFromCodeCoverage]

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
        [Required]
        public string Tipo { get; set; } = string.Empty;
    }
    public class LoginResult
    {
        public string? Token { get; set; }
        public bool DoisFatoresNecessario { get; set; }
    }
}
