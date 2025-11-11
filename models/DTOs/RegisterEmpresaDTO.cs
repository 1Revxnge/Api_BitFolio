namespace BitFolio.models.DTOs
{
    public class RegisterEmpresaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string? RazaoSocial { get; set; }
        public string CNPJ { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }
}
