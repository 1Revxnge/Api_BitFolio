using System.Diagnostics.CodeAnalysis;

namespace BitFolio.models.DTOs
{
    [ExcludeFromCodeCoverage]

    public class FiltroVagaDTO
    {
        public string? PalavrasChave { get; set; }
        public double? Proximidade { get; set; }
        public string? Linguagens { get; set; }
        public string? Experiencia { get; set; }
        public string? Area { get; set; }
        public string? Modelo { get; set; }
        public int Page { get; set; } = 1;
        public int Take { get; set; } = 10;
    }
}
