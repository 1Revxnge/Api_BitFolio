using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models
{
    [ExcludeFromCodeCoverage]

    public class LogRecrutador
    {
        public Guid LogId { get; set; }
        public Guid RecrutadorId { get; set; }
        public Recrutador? Recrutador { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime? DtAcao { get; set; }
    }

    public class LogEndereco
    {
        public Guid LogId { get; set; }
        public string Acao { get; set; }
        public DateTime? DtAcao { get; set; }
    }
}
