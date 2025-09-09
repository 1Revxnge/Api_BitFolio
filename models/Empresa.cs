
namespace ApiJobfy.models
{ 
    public class Empresa
    {
        public Guid EmpresaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? RazaoSocial { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime? DataCadastro { get; set; }
        public Guid? EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        // Relacionamentos
        public ICollection<Recrutador> Recrutadores { get; set; } = new List<Recrutador>();
        public ICollection<Vaga> Vagas { get; set; } = new List<Vaga>();
    }
}


