namespace ApiJobfy.Services
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string para, string assunto, string corpo);
    }
}