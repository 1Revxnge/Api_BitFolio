using System.ComponentModel.DataAnnotations;

namespace ApiJobfy.models
{
    public class Administrador
    {
        public Guid AdminId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime? UltimoAcesso { get; set; }
        // teste
        public ICollection<LogAdministrador> Logs { get; set; } = new List<LogAdministrador>();
    }
}
