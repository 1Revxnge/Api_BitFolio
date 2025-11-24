using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services.IService
{
    public interface IEmailService
    {
        [ExcludeFromCodeCoverage]

        Task EnviarEmailAsync(string para, string assunto, string corpo);
    }
}