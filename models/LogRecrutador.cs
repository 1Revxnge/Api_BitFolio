﻿namespace ApiJobfy.models
{
    public class LogRecrutador
    {
        public Guid LogId { get; set; }
        public Guid RecrutadorId { get; set; }
        public Recrutador? Recrutador { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime? DtAcao { get; set; }
    }
}
