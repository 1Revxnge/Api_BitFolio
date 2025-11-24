using ApiJobfy.models;
using System.Diagnostics.CodeAnalysis;

namespace BitFolio.models
{

    public class HistoricoCandidatura
    {

        public Guid HistoricoId { get; set; }
        public Guid CandidatoId { get; set; }
        public Guid VagaId { get; set; }
        public StatusVaga Status { get; set; }
        public DateTime DtCandidatura { get; set; }
        public DateTime DtAtualizacao { get; set; }
        public Candidato? Candidato { get; set; }
        public Vaga? Vaga { get; set; }
    }

    public enum StatusVaga
    {
        CVRecebido = 0,
        CVRevisado = 1,
        CVPreSelecionado = 2,
        CVSelecionado = 3,
        CVNaoSelecionado = 4
    }
}
