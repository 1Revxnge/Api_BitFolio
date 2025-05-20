namespace ApiJobfy.models
{
    public class LogFuncionarios
    {
        public int Id { get; set; }
        public int FuncionarioId { get; set; }
        public string Acao { get; set; }
        public DateTime DtAcao { get; set; }
        public Funcionario Funcionario { get; set; }

    }
}
