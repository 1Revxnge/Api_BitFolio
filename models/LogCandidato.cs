using System;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models
{
    [ExcludeFromCodeCoverage]

        public class LogCandidato
        {
            public Guid LogId { get; set; }
            public Guid CandidatoId { get; set; }
            public Candidato? Candidato { get; set; }
            public string Acao { get; set; } = string.Empty;
            public DateTime? DtAcao { get; set; }
        }
}
