namespace ApiJobfy.models
{
    public class Empresas
    {
        public int Id { get; set; } 
        public string Nome { get; set; } 
        public string Cnpj { get; set; } 
        public string Email { get; set; } 
        public string Descricao { get; set; } 
        public List<Endereco> Endereco { get; set; } 
        public string LogoUrl { get; set; } 
        public bool Ativo { get; set; }
        public DateTime? DtAprovacao { get; set; } 
        public DateTime DtCadastro { get; set; } 

        // Método de anonimização
        public void Anonimizar()
        {
            Nome = "Anonimizado";
            Cnpj = "00.000.000/0000-00"; // Exemplo de CNPJ "zerado"
            Email = $"anonimizado{Id}@example.com";

   
        }
    }
}
