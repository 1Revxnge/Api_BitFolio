namespace ApiJobfy.models
{
    public class Permissoes
    {
        public int Id { get; set; }
        public int CandidatoId { get; set; } 
        public string NomePermissao { get; set; }
        public DateTime DtPermissao { get; set; }
        public Candidato Candidato { get; set; }

    }
}
