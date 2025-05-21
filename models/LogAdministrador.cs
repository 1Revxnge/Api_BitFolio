namespace ApiJobfy.models
{
    public class LogAdministrador
    {
        public int Id { get; set; }
        public int AdministradorId { get; set; }
        public string Acao { get; set; }
        public DateTime DtAcao { get; set; }
        public Administrador Administrador { get; set; }

    }
}
