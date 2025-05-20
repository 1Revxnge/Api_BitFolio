using System;

namespace ApiJobfy.models
{
    public class LogCandidato
    {
        public int Id { get; set; }
        public int CandidatoId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime DataAcao { get; set; } = DateTime.UtcNow;
        public Candidato Candidato { get; set; }
    }
}
