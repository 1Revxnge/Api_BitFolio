using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using BitFolio.models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services
{
    [ExcludeFromCodeCoverage]

    public class EnderecoService : IEnderecoService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private static readonly string _templateAlteracaoEndereco = "https://bitfolio-s3.s3.sa-east-1.amazonaws.com/AlteracaoEnderecoEmpresa.html";

        public EnderecoService(AppDbContext dbContext, IConfiguration configuration, IEmailService emailService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<Endereco> AddEnderecoAsync(Endereco endereco, Guid? candidatoId = null, Guid? empresaId = null)
        {
            _dbContext.Enderecos.Add(endereco);
            await _dbContext.SaveChangesAsync();

            if (candidatoId.HasValue)
            {
                var candidato = await _dbContext.Candidatos.FindAsync(candidatoId.Value);
                if (candidato != null)
                {
                    candidato.EnderecoId = endereco.EnderecoId;
                    await _dbContext.SaveChangesAsync();
                }
            }
            else if (empresaId.HasValue)
            {
                var empresa = await _dbContext.Empresas.FindAsync(empresaId.Value);
                if (empresa != null)
                {
                    empresa.EnderecoId = endereco.EnderecoId;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return endereco;
        }

        public async Task<bool> SolicitarAlteracaoEnderecoAsync(
     SolicitacaoEndereco solicitacao,
     Guid funcionarioId)
        {
            // 1. Buscar empresa e endereço atual
            var empresa = await _dbContext.Empresas
                .Include(e => e.Endereco)
                .FirstOrDefaultAsync(e => e.EnderecoId == solicitacao.EnderecoId);

            if (empresa == null)
                return false;

            var enderecoAtual = empresa.Endereco;
            if (enderecoAtual == null)
                return false;

            // 2. Buscar o funcionário que solicitou
            var funcionario = await _dbContext.Recrutadores
                .FirstOrDefaultAsync(f => f.RecrutadorId == funcionarioId);

            if (funcionario == null)
                return false;

            var nomeFuncionario = funcionario.Nome;

            // 3. Salvar a solicitação
            solicitacao.SolicitacaoId = Guid.NewGuid();
            solicitacao.DataSolicitacao = DateTime.UtcNow;
            solicitacao.Aprovado = false;

            _dbContext.SolicitacoesEndereco.Add(solicitacao);
            await _dbContext.SaveChangesAsync();

            // 4. Criar log de solicitação de alteração de endereço
            var logEndereco = new LogEndereco
            {
                LogId = Guid.NewGuid(),
                Acao = $"O funcionário {nomeFuncionario} solicitou uma alteração no endereço.",
                DtAcao = DateTime.UtcNow
            };

            _dbContext.LogEnderecos.Add(logEndereco);
            await _dbContext.SaveChangesAsync();

            // 5. Enviar e-mail
            await EnviarEmailSolicitacaoEnderecoAsync(
                empresa,
                enderecoAtual,
                solicitacao,
                nomeFuncionario
            );

            return true;
        }




        private async Task EnviarEmailSolicitacaoEnderecoAsync(
     Empresa empresa,
     Endereco enderecoAtual,
     SolicitacaoEndereco solicitacao,
     string nomeFuncionario)
        {
            var http = new HttpClient();
            var template = await http.GetStringAsync(_templateAlteracaoEndereco);

            string FormatarCep(string cep)
            {
                if (string.IsNullOrWhiteSpace(cep))
                    return "-";

                var numeros = new string(cep.Where(char.IsDigit).ToArray());

                if (numeros.Length == 8)
                    return $"{numeros.Substring(0, 5)}-{numeros.Substring(5, 3)}";

                return cep; 
            }

            // Preenche placeholders
            template = template
                .Replace("{{NOME_RESPONSAVEL}}", empresa.Nome)
                .Replace("{{NOME_FUNCIONARIO}}", nomeFuncionario)

                // Endereço atual
                .Replace("{{CEP_ANTIGO}}", FormatarCep(enderecoAtual.Cep))
                .Replace("{{LOGRADOURO_ANTIGO}}", enderecoAtual.Rua)
                .Replace("{{NUMERO_ANTIGO}}", enderecoAtual.Numero)
                .Replace("{{COMPLEMENTO_ANTIGO}}", enderecoAtual.Complemento ?? "-")
                .Replace("{{BAIRRO_ANTIGO}}", enderecoAtual.Bairro)
                .Replace("{{CIDADE_ANTIGA}}", enderecoAtual.Cidade)
                .Replace("{{ESTADO_ANTIGO}}", enderecoAtual.Estado)

                // Novo endereço
                .Replace("{{CEP_NOVO}}", FormatarCep(solicitacao.CepNovo))
                .Replace("{{LOGRADOURO_NOVO}}", solicitacao.RuaNova)
                .Replace("{{NUMERO_NOVO}}", solicitacao.NumeroNovo)
                .Replace("{{COMPLEMENTO_NOVO}}", solicitacao.ComplementoNovo ?? "-")
                .Replace("{{BAIRRO_NOVO}}", solicitacao.BairroNovo)
                .Replace("{{CIDADE_NOVA}}", solicitacao.CidadeNova)
                .Replace("{{ESTADO_NOVO}}", solicitacao.EstadoNovo)
                .Replace("{{DATA_SOLICITACAO}}",
                    TimeZoneInfo.ConvertTimeFromUtc(solicitacao.DataSolicitacao, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))
                    .ToString("dd/MM/yyyy HH:mm"))
              // Link de aprovação
                .Replace("{{LINK_APROVACAO}}",
                $"https://bitfolio-s3.s3.sa-east-1.amazonaws.com/ConfirmacaoEnderecoEmpresa.html?token={solicitacao.SolicitacaoId}");

            // Envia e-mail
            await _emailService.EnviarEmailAsync(
                empresa.Email,
                "Solicitação de Alteração de Endereço",
                template
            );
        }




        public async Task<bool> UpdateEnderecoAsync(Endereco endereco)
        {
            var existingEndereco = await _dbContext.Enderecos
                .FirstOrDefaultAsync(e => e.EnderecoId == endereco.EnderecoId);

            if (existingEndereco == null)
            {
                return false;
            }
           

                existingEndereco.Rua = endereco.Rua;
                existingEndereco.Numero = endereco.Numero;
                existingEndereco.Complemento = endereco.Complemento;
                existingEndereco.Bairro = endereco.Bairro;
                existingEndereco.Cidade = endereco.Cidade;
                existingEndereco.Estado = endereco.Estado;
                existingEndereco.Cep = endereco.Cep;
                existingEndereco.Latitude = endereco.Latitude;
                existingEndereco.Longitude = endereco.Longitude;

                await _dbContext.SaveChangesAsync();
                return true;
            
        }

        public async Task<Endereco> GetEnderecoByIdAsync(Guid id)
        {
            var endereco = await _dbContext.Enderecos
                .FirstOrDefaultAsync(e => e.EnderecoId == id);

            if (endereco == null)
                throw new Exception("Endereço não encontrado.");

            return endereco;
        }
    }
}