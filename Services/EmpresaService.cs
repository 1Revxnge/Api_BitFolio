using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using ApiJobfy.models;
using ApiJobfy.Data;
using ApiJobfy.Services.IService;
using BitFolio.models;

namespace ApiJobfy.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _emailService;
        private static readonly string _templateAlteracaoEmpresa = "https://bitfolio-s3.s3.sa-east-1.amazonaws.com/AlteracaoDadosEmpresa.html";
        public EmpresaService(AppDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task<IEnumerable<Empresa>> GetEmpresasAsync(int page, int pageSize)
        {
            return await _dbContext.Empresas
                .Where(e => e.Ativo) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Empresa?> GetEmpresaByIdAsync(Guid id)
        {
            return await _dbContext.Empresas
                .Include(e => e.Endereco)       
                .Include(e => e.Vagas)         
                .Include(e => e.Recrutadores)  
                .FirstOrDefaultAsync(e => e.EmpresaId == id);
        }

        public async Task<Empresa> AddEmpresaAsync(Empresa empresa)
        {
            empresa.Ativo = false;
            empresa.DataCadastro = DateTime.UtcNow;
            _dbContext.Empresas.Add(empresa);
            await _dbContext.SaveChangesAsync();
            return empresa;
        }

        public async Task<bool> UpdateEmpresaAsync(Empresa empresa)
        {
            var existingEmpresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.EmpresaId == empresa.EmpresaId);

            if (existingEmpresa == null)
                return false;

            existingEmpresa.Nome = empresa.Nome;
            existingEmpresa.Email = empresa.Email;
            existingEmpresa.CNPJ = empresa.CNPJ;
            existingEmpresa.Descricao = empresa.Descricao;
            existingEmpresa.LogoUrl = empresa.LogoUrl;
            existingEmpresa.EnderecoId = empresa.EnderecoId;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ExistsByCnpjAsync(string cnpj)
        {
            string clean = new string(cnpj.Where(char.IsDigit).ToArray());

            var cnpjs = await _dbContext.Empresas
                .Select(e => e.CNPJ)
                .ToListAsync(); 

            return cnpjs.Any(c =>
            {
                string dbClean = new string(c.Where(char.IsDigit).ToArray());
                return dbClean == clean;
            });
        }

        public async Task<IEnumerable<Empresa>> GetTodasEmpresasAsync(int page, int take)
        {
            return await _dbContext.Empresas
                .OrderBy(e => e.Nome)
                .Skip((page - 1) * take)
                .Take(take)
                .ToListAsync();
        }
        public async Task<Empresa?> AprovarEmpresaAsync(Guid empresaId)
        {
            var empresa = await _dbContext.Empresas.FindAsync(empresaId);

            if (empresa == null)
                return null;

            empresa.Ativo = true;

            await _dbContext.SaveChangesAsync();

            return empresa;
        }

        public async Task<bool> ReprovarEmpresaAsync(Guid empresaId)
        {
            var empresa = await _dbContext.Empresas
                .Include(e => e.Recrutadores)
                .FirstOrDefaultAsync(e => e.EmpresaId == empresaId);

            if (empresa == null)
                return false;

            // 1. Buscar todas as vagas criadas pela empresa
            var vagas = await _dbContext.Vagas
                .Where(v => v.EmpresaId == empresaId)
                .ToListAsync();

            if (vagas.Any())
            {
                var vagaIds = vagas.Select(v => v.VagaId).ToList();

                var historicos = await _dbContext.HistoricoCandidaturas
                    .Where(h => vagaIds.Contains(h.VagaId))
                    .ToListAsync();

                if (historicos.Any())
                    _dbContext.HistoricoCandidaturas.RemoveRange(historicos);

                // 3. Excluir candidatos relacionados às vagas
                var candidatosVagas = await _dbContext.CandidatoVagas
                    .Where(cv => vagaIds.Contains(cv.VagaId))
                    .ToListAsync();

                if (candidatosVagas.Any())
                    _dbContext.CandidatoVagas.RemoveRange(candidatosVagas);

                // 4. Excluir as vagas
                _dbContext.Vagas.RemoveRange(vagas);
            }

            // 5. Remover recrutadores vinculados a essa empresa
            var recrutadores = await _dbContext.Recrutadores
                .Where(r => r.EmpresaId == empresaId)
                .ToListAsync();

            if (recrutadores.Any())
            {
                _dbContext.Recrutadores.RemoveRange(recrutadores);
            }

            // 6. Remover a empresa
            _dbContext.Empresas.Remove(empresa);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalEmpresasAsync()
        {
            return await _dbContext.Empresas.CountAsync();
        }

        public async Task<bool> AprovarSolicitacaoEnderecoAsync(Guid solicitacaoId)
        {
            var solicitacao = await _dbContext.SolicitacoesEndereco
                .FirstOrDefaultAsync(s => s.SolicitacaoId == solicitacaoId);

            if (solicitacao == null || solicitacao.Aprovado)
                return false;

            var empresa = await _dbContext.Empresas
                .Include(e => e.Endereco)
                .FirstOrDefaultAsync(e => e.EnderecoId == solicitacao.EnderecoId);

            if (empresa == null)
                return false;

            // Atualiza o endereço
            var endereco = empresa.Endereco!;
            endereco.Cep = solicitacao.CepNovo;
            endereco.Rua = solicitacao.RuaNova;
            endereco.Numero = solicitacao.NumeroNovo;
            endereco.Complemento = solicitacao.ComplementoNovo;
            endereco.Bairro = solicitacao.BairroNovo;
            endereco.Cidade = solicitacao.CidadeNova;
            endereco.Estado = solicitacao.EstadoNovo;
            endereco.Latitude = solicitacao.LatitudeNova;
            endereco.Longitude = solicitacao.LongitudeNova;

            _dbContext.SolicitacoesEndereco.Remove(solicitacao);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SolicitarAlteracaoEmpresaAsync(
     SolicitacaoEmpresa solicitacao,
     Guid funcionarioId)
        {
            // 1. Buscar empresa atual
            var empresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.EmpresaId == solicitacao.EmpresaId);

            if (empresa == null)
                return false;

            // 2. Buscar o funcionário que solicitou a alteração
            var funcionario = await _dbContext.Recrutadores
                .FirstOrDefaultAsync(f => f.RecrutadorId == funcionarioId);

            if (funcionario == null)
                return false;

            var nomeFuncionario = funcionario.Nome;

            // 3. Salvar a solicitação
            solicitacao.SolicitacaoId = Guid.NewGuid();
            solicitacao.DataSolicitacao = DateTime.UtcNow;
            solicitacao.Aprovado = false;

            _dbContext.SolicitacoesEmpresa.Add(solicitacao);
            await _dbContext.SaveChangesAsync();

            // 4. Enviar e-mail
            await EnviarEmailSolicitacaoEmpresaAsync(
                empresa,
                solicitacao,
                nomeFuncionario
            );

            return true;
        }

        private async Task EnviarEmailSolicitacaoEmpresaAsync(
            Empresa empresa,
            SolicitacaoEmpresa solicitacao,
            string nomeFuncionario)
        {
            var http = new HttpClient();
            var template = await http.GetStringAsync(_templateAlteracaoEmpresa);

            // Preenche placeholders
            template = template
                .Replace("{{NOME_RESPONSAVEL}}", nomeFuncionario)
                .Replace("{{NOME_EMPRESA}}", empresa.Nome)
                // Dados antigos
                .Replace("{{NOME_ANTIGO}}", empresa.Nome)
                .Replace("{{RAZAO_SOCIAL_ANTIGA}}", empresa.RazaoSocial)
                .Replace("{{DESCRICAO_ANTIGO}}", empresa.Descricao ?? "-")
                // Dados novos
                .Replace("{{NOME_NOVO}}", solicitacao.NomeNovo ?? "-")
                .Replace("{{RAZAO_SOCIAL_NOVA}}", solicitacao.RazaoSocialNova ?? "-")
                .Replace("{{DESCRICAO_NOVO}}", solicitacao.DescricaoNova ?? "-")
                .Replace("{{DATA_SOLICITACAO}}",
                    TimeZoneInfo.ConvertTimeFromUtc(
                        solicitacao.DataSolicitacao,
                        TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")
                    ).ToString("dd/MM/yyyy HH:mm"))
               .Replace("{{LINK_APROVACAO}}",
                $"https://bitfolio-s3.s3.sa-east-1.amazonaws.com/ConfirmacaoDadosEmpresa.html?token={solicitacao.SolicitacaoId}");

            // Envia e-mail
            await _emailService.EnviarEmailAsync(
                empresa.Email,
                "Solicitação de Alteração de Dados da Empresa",
                template
            );
        }
        public async Task<bool> AprovarSolicitacaoEmpresaAsync(Guid solicitacaoId)
        {
            var solicitacao = await _dbContext.SolicitacoesEmpresa
                .FirstOrDefaultAsync(s => s.SolicitacaoId == solicitacaoId);

            if (solicitacao == null || solicitacao.Aprovado)
                return false;

            var empresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.EmpresaId == solicitacao.EmpresaId);

            if (empresa == null)
                return false;

            // Aplica alterações
            empresa.Nome = solicitacao.NomeNovo ?? empresa.Nome;
            empresa.RazaoSocial = solicitacao.RazaoSocialNova ?? empresa.RazaoSocial;
            empresa.Descricao = solicitacao.DescricaoNova ?? empresa.Descricao;


            _dbContext.SolicitacoesEmpresa.Remove(solicitacao); 

            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}
