using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models
{
    [ExcludeFromCodeCoverage]

    public class Curriculo
    {
        public Guid CurriculoId { get; set; }
        public string? Experiencias { get; set; }
        public string? Tecnologias { get; set; }
        public string? CompetenciasTecnicas { get; set; }
        public string? Idiomas { get; set; }
        public string? Certificacoes { get; set; }

        // Relacionamento 1:1 com Candidato
        public Candidato? Candidato { get; set; }
    }

}
