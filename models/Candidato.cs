using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiJobfy.models
{
    public class Candidato
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)] 
        public DateOnly DtNascimento { get; set; }

        [Required]
        public byte[] CurriculoCriptografado { get; set; } = Array.Empty<byte>();
        [Required]
        public string Status { get; set; } 

        public DateTime DtCriacao { get; set; }
        public DateTime DtAtualizacao { get; set; }
        [Required]
        public bool Ativo { get; set; }

        public ICollection<LogCandidato> LogCandidatos { get; set; }
        public ICollection<Permissoes> Permissoes { get; set; }
        // Anonimizar currículo (limpar dados sensíveis eventualmente)
        public virtual void Anonimizar()
        {
            // Anonimizar dados pessoais
            Nome = "Anonimizado";
            Email = $"anonimizado{Id}@example.com";
            Telefone = "0000000000";

            // Anonimizar ou limpar currículo criptografado
            CurriculoCriptografado = Array.Empty<byte>(); // Limpa o currículo (dados sensíveis)
        }
    }
}
