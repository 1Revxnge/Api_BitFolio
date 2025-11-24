using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models
{
    [ExcludeFromCodeCoverage]

    public class VagaFavorita
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Candidato? Candidato { get; set; }
        public Guid VagaId { get; set; }
        public Vaga? Vaga { get; set; }
    }
}
