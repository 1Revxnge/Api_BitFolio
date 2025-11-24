using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models.DTOs
{
    [ExcludeFromCodeCoverage]

    public class RegisterCandidatoDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public DateOnly DataNascimento { get; set; }

        [Required]
        [Phone]
        public string Telefone { get; set; } = string.Empty;

    }
}
