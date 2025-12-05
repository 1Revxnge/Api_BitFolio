using ApiJobfy.models;

namespace BitFolio.models
{
    public class SolicitacaoEmpresa
    {

        public Guid SolicitacaoId { get; set; }
        public Guid EmpresaId { get; set; }

        // Campos alteráveis
        public string? NomeNovo { get; set; }
        public string? RazaoSocialNova { get; set; }
        public string? DescricaoNova { get; set; }

        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
        public bool Aprovado { get; set; } = false;

        // Relacionamento
        public Empresa Empresa { get; set; }

    }
}
