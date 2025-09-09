using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiJobfy.models
{
    public class Candidato
    {
        public Guid CandidatoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public DateOnly? DataNascimento { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public bool Ativo { get; set; } = true;

        public Guid? EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        public Guid? CurriculoId { get; set; }
        public Curriculo? Curriculo { get; set; }

        // Relacionamentos N:N e logs
        public ICollection<CandidatoVaga> CandidatoVagas { get; set; } = new List<CandidatoVaga>();
        public ICollection<VagaFavorita> VagasFavoritas { get; set; } = new List<VagaFavorita>();
        public ICollection<LogCandidato> Logs { get; set; } = new List<LogCandidato>();
    }

}
