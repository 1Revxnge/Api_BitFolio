using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiJobfy.models
{
    public class Funcionario 
    {
        public int Id { get; set; } 
        public int NegocioId { get; set; } 

        public string Nome { get; set; } 
        public string Email { get; set; } 
        public string Senha { get; set; }
        public DateOnly DtNascimento { get; set; } 
        public decimal Salario { get; set; } 
        public string Cargo { get; set; } 
        public string Telefone { get; set; } 
        public string StatusFunc { get; set; }
        public DateOnly DtAdmissao { get; set; } 
        public DateOnly? DtDemissao { get; set; } 
        public DateTime DtCriacao { get; set; } 
        public DateTime DtAtualizacao { get; set; }
        public bool Ativo { get; set; }

        public ICollection<LogFuncionarios> LogFuncionarios { get; set; }

        // Método independente de anonimização
        public void Anonimizar()
        {
            // Anonimiza dados pessoais (nome, email, telefone)
            Nome = "Anonimizado";
            Email = $"anonimizado{Id}@example.com";
            Telefone = "0000000000";

            // Anonimiza campos específicos do funcionário
            Salario = 0m;  
            Cargo = "Anonimizado";
            NegocioId = 0;
        }
    }
}