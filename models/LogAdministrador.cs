using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.models
{
    [ExcludeFromCodeCoverage]

    public class LogAdministrador
    {
        public Guid LogId { get; set; }
        public Guid AdminId { get; set; }
        public Administrador? Administrador { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime? DtAcao { get; set; }
    }
}
