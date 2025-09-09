using System.ComponentModel.DataAnnotations;

namespace ApiJobfy.models.DTOs
{
    public class RegisterAdminDto
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
        [Phone]
        public string Telefone { get; set; } = string.Empty;

        [Required]
        public bool Aprovado { get; set; }
    }
}
