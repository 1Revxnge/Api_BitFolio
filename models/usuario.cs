using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiJobfy.models
{
    public abstract class Usuario
    {
        [Key]
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
        public DateOnly DataNascimento { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        // Método para esconder/anonimizar dados no caso de soft delete
        public virtual void Anonimizar()
        {
            Nome = "Anonimizado";
            Email = $"anonimizado{Id}@example.com";
            Telefone = "0000000000";
        }
    }
}
