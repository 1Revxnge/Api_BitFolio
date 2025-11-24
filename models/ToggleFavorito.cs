using System.Diagnostics.CodeAnalysis;

namespace BitFolio.models
{
    [ExcludeFromCodeCoverage]

    public class ToggleFavorito
    {
        public Guid CandidatoId { get; set; }
        public Guid VagaId { get; set; }
    }
}
