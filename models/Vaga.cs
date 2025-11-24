    using BitFolio.models;

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

            public string? Tecnologias { get; set; }
            public string? Area { get; set; }
            public decimal? Salario { get; set; }

            public Guid EmpresaId { get; set; }
            public Empresa? Empresa { get; set; }

            public ICollection<CandidatoVaga> CandidatoVagas { get; set; } = new List<CandidatoVaga>();
            public ICollection<VagaFavorita> VagasFavoritas { get; set; } = new List<VagaFavorita>();
            public ICollection<HistoricoCandidatura> Historicos { get; set; } = new List<HistoricoCandidatura>();
        }
    }