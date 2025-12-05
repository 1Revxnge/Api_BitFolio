namespace BitFolio.models.DTOs
{
    public class AlterarSenhaDto
    {
        public string Email { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmacaoNovaSenha { get; set; }
    }
}
