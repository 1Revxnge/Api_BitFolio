namespace ApiJobfy.models
{
    public class Vagas
    {
        public int Id { get; set; } 
        public int NegocioId { get; set; } 
        public string Titulo { get; set; } 
        public string Descricao { get; set; }
        public List<string> Requisitos { get; set; } = new List<string>(); 
        public List<string> Beneficios { get; set; } = new List<string>();
        public string Escolaridade { get; set; } 
        public string Modalidade { get; set; }
        public int Quantidade { get; set; } 
        public string Localizacao { get; set; }
        public TimeOnly DtInicio { get; set; } 
        public TimeOnly DtFim { get; set; } 
        public bool Ativo { get; set; } 
        public DateTime DtCadastro { get; set; }
        public DateTime DtAtualizacao { get; set; }
        public Empresas Empresa { get; set; }

        public void Anonimizar()
        {
            // Dados pessoais (localização) são anonimizado
            Localizacao = "Anonimizado";

           
        }
    }
}