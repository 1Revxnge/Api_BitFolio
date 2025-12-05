namespace BitFolio.models
{
    public class SolicitacaoEndereco
    {
        public Guid SolicitacaoId { get; set; }
        public Guid EnderecoId { get; set; }
        public string RuaNova { get; set; }
        public string NumeroNovo { get; set; }
        public string ComplementoNovo { get; set; }
        public string BairroNovo { get; set; }
        public string CidadeNova { get; set; }
        public string EstadoNovo { get; set; }
        public string CepNovo { get; set; }
        public double? LatitudeNova { get; set; }
        public double? LongitudeNova { get; set; }

        public DateTime DataSolicitacao { get; set; }
        public bool Aprovado { get; set; }
    }
}
