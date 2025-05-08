namespace ApiJobfy.models
{
    public class Endereco
    {
        public int Id { get; set; } 
        public string Rua { get; set; } 
        public string Numero { get; set; } 
        public string? Complemento { get; set; }
        public string Bairro { get; set; } 
        public string Cidade { get; set; } 
        public string Estado { get; set; } 
        public string Cep { get; set; }
        public DateTime DtCriacao { get; set; } 
        public DateTime DtAtualizacao { get; set; } 

        // Método de anonimização
        public void Anonimizar()
        {
            // Anonimiza dados pessoais (rua, numero, complemento, bairro, cep)
            Rua = "Anonimizado";
            Numero = "000";
            Complemento = "Anonimizado";
            Bairro = "Anonimizado";
            Cep = "00000-000"; 
            Cidade = "Anonimizada";
            Estado = "Anonimizado";
        }
    }
}