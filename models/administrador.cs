using System.ComponentModel.DataAnnotations;

namespace ApiJobfy.models
{
    public class Administrador 
    {
        public int Id { get; set; } 
        public int NegocioId { get; set; } 
        public string Nome { get; set; } 
        public string Email { get; set; } 
        public string Senha { get; set; }
        public decimal Salario { get; set; }
        public string Telefone { get; set; }
        public TimeOnly DtNascimento { get; set; } 
        public string Cargo { get; set; } 
        public bool Aprovado { get; set; } 
        public TimeOnly? DtAprovacao { get; set; } 
        public DateTime DtCadastro { get; set; }

        public ICollection<LogAdministrador> LogAdministradores { get; set; }

        // Método de anonimização
        public void Anonimizar()
        {
            // Anonimiza dados pessoais (nome, email, telefone, nascimento)
            Nome = "Anonimizado";
            Email = $"anonimizado{Id}@example.com";
            Telefone = "0000000000";
            Salario = 0m;
            NegocioId = 0;


        }
    }
}
