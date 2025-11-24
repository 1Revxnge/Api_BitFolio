using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models.DTOs
{
    [ExcludeFromCodeCoverage]

    public class RedefinirSenhaDto
    {
        public string Email { get; set; }
        public string Codigo { get; set; }
        public string NovaSenha { get; set; }
    }
}
