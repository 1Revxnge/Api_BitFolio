using System;
using System.ComponentModel.DataAnnotations;

namespace ApiJobfy.models
{
    public class Funcionario : Usuario
    {
        [Required]
        [MaxLength(100)]
        public string Cargo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Loja { get; set; } = string.Empty;

        public DateTime DataAdmissao { get; set; } = DateTime.UtcNow;

        public DateTime? DataDemissao { get; set; } = null;

        public override void Anonimizar()
        {
            base.Anonimizar();
            Cargo = "Anonimizado";
            Loja = "Anonimizado";
        }
    }
}
