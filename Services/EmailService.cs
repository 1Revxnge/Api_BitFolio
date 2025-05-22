using System.Threading.Tasks;
using ApiJobfy.Services.IService;
using Microsoft.Extensions.Logging;

namespace ApiJobfy.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        // Implementação simples de envio de e-mail (mock)
        public Task EnviarEmailAsync(string para, string assunto, string corpo)
        {
            _logger.LogInformation($"Simulando envio de email para {para} com assunto: {assunto}");
            // Aqui você colocaria integração real com serviço de email
            return Task.CompletedTask;
        }
    }
}
