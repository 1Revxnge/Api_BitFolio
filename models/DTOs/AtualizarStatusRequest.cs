using System.Diagnostics.CodeAnalysis;

namespace BitFolio.models.DTOs
{
    public class AtualizarStatusRequest
    {
        public Guid CandidatoId { get; set; }
        public Guid VagaId { get; set; }
        public StatusVaga NovoStatus { get; set; }
    }
}
