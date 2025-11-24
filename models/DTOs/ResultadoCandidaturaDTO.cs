namespace BitFolio.models.DTOs
{

    public class ResultadoCandidaturaDTO
    {
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public int? Codigo { get; set; } 
        public Guid? HistoricoId { get; set; }
    }
}
