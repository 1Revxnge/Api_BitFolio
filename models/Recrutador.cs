using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiJobfy.models
{
    public class Recrutador
    {
        public Guid RecrutadorId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public Guid? EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public ICollection<LogRecrutador> Logs { get; set; } = new List<LogRecrutador>();
    }
}