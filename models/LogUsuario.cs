using System;

namespace ApiJobfy.models
{
    public class LogUsuario
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime DataAcao { get; set; } = DateTime.UtcNow;
    }
}
