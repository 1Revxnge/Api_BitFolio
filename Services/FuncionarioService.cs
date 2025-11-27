using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services
{
    [ExcludeFromCodeCoverage]

    public class FuncionarioService : IFuncionarioService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FuncionarioService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Recrutador>> GetFuncionariosAsync(int page, int take)
        {
            int skip = take * (page - 1);

            return await _dbContext.Recrutadores
                .Where(f => f.Ativo == true)
                .Include(f => f.Empresa)
                .OrderBy(f => f.Nome)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalFuncionariosAsync()
        {
            return await _dbContext.Recrutadores.CountAsync(f => f.Ativo == true);
        }


        public async Task<Recrutador?> GetFuncionarioByIdAsync(Guid id)
        {
            return await _dbContext.Recrutadores
                .FirstOrDefaultAsync(f => f.RecrutadorId == id);
        }

        public async Task<bool> UpdateFuncionarioAsync(Recrutador funcionario)
        {
            var existingFuncionario = await _dbContext.Recrutadores
                .FirstOrDefaultAsync(f => f.RecrutadorId == funcionario.RecrutadorId);

            if (existingFuncionario == null)
                return false; 

            existingFuncionario.Nome = funcionario.Nome;
            existingFuncionario.Email = funcionario.Email;
            existingFuncionario.Telefone = funcionario.Telefone;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Recrutador> AddFuncionarioAsync(Recrutador funcionario)
        {
            _dbContext.Recrutadores.Add(funcionario);
            await _dbContext.SaveChangesAsync();

            return funcionario;
        }
        public async Task<bool> DeleteFuncionarioAsync(Guid id)
        {
            var funcionario = await _dbContext.Recrutadores
                .FirstOrDefaultAsync(f => f.RecrutadorId == id);

            if (funcionario == null)
                return false;

            _dbContext.Recrutadores.Remove(funcionario);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetTotalLogsRecrutadorAsync(Guid recrutadorId)
        {
            return await _dbContext.LogRecrutadores
         .Where(l =>
             l.RecrutadorId == recrutadorId &&
             l.Acao != "Login bem-sucedido" &&
             l.Acao != "Login falhou"
         )
         .CountAsync();
        }

        public async Task<List<LogRecrutador>> GetLogsRecrutadorAsync(Guid recrutadorId, int page, int pageSize)
        {
            return await _dbContext.LogRecrutadores
                .Where(l =>
            l.RecrutadorId == recrutadorId &&
            l.Acao != "Login bem-sucedido" &&
            l.Acao != "Login falhou"
        )
                .OrderByDescending(l => l.DtAcao)
                .Skip((page - 1) * pageSize)
            .Take(pageSize)
                .Select(l => new LogRecrutador
                {
                    LogId = l.LogId,
                    Acao = l.Acao,
                    DtAcao = l.DtAcao
                })
                .ToListAsync();
        }
    }
}