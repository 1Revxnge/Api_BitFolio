namespace BitFolio.models.DTOs
{
    public class VagaDTO
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
        public bool Ativo { get; set; }
        public string? Tecnologias { get; set; }
        public string? Area { get; set; }
        public decimal? Salario { get; set; }
        public Guid EmpresaId { get; set; }
        public string? EmpresaNome { get; set; }

        // Endereço resumido
        public string? Endereco { get; set; }

        // Distância em km do candidato
        public double? Distancia { get; set; }
    }

}
