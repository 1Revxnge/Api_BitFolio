namespace ApiJobfy.models
{
    public class CandidatoVaga
    {
        public Guid Id { get; set; }
        public Guid CandidatoId { get; set; }
        public Candidato? Candidato { get; set; }
        public Guid VagaId { get; set; }
        public Vaga? Vaga { get; set; }
    }
}
