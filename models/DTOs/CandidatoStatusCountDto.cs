namespace BitFolio.models.DTOs
{
    public class CandidatoStatusCountDto
    {
        public int Total { get; set; }
        public int EmAnalise { get; set; }
        public int Revisado { get; set; }
        public int Entrevista { get; set; }
        public int Aprovados { get; set; }
        public int Rejeitados { get; set; }
    }
}
