namespace ApiJobfy.models
{
    public class TokenTemporario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public TipoToken Tipo { get; set; } // Enum 
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime ExpiraEm { get; set; }
        public bool Utilizado { get; set; } = false;
    }   
    public enum TipoToken
    {
        RecuperacaoSenha = 1,
        ValidacaoEmail = 2,
        DoisFatores = 3
    }
}
