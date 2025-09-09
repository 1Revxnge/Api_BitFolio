namespace ApiJobfy.models
{
    public class Vaga
    {
        public Guid VagaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Nivel { get; set; }
        public string? Escolaridade { get; set; }
        public string? Modelo { get; set; } 
        public DateTime? DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string? Requisitos { get; set; }
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;
        public Guid EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }

        // Relacionamentos
        public ICollection<CandidatoVaga> CandidatoVagas { get; set; } = new List<CandidatoVaga>();
        public ICollection<VagaFavorita> Favoritos { get; set; } = new List<VagaFavorita>();
    }
}