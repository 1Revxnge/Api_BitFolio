using System.Threading.Tasks;
using ApiJobfy.Services.IService;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
namespace ApiJobfy.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task EnviarEmailAsync(string para, string assunto, string corpo)
        {
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("JobFY", "contato.jobfy@gmail.com")); 
            mensagem.To.Add(new MailboxAddress("", para));
            mensagem.Subject = assunto;

            mensagem.Body = new TextPart("html") 
            {
                Text = corpo
            };

            using (var cliente = new SmtpClient())
            {
                try
                {
                    await cliente.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await cliente.AuthenticateAsync("contato.jobfy@gmail.com", "pqeevqmgjeuoerml"); 

                    await cliente.SendAsync(mensagem);
                    await cliente.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar e-mail.");
                    throw;
                }
            }
        }
    }
}

