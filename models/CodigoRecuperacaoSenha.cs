namespace ApiJobfy.models
{
    public class CodigoRecuperacaoSenha
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime ExpiraEm { get; set; }
        public bool Utilizado { get; set; } = false;
    }
}
