using ApiJobfy.Data;
using ApiJobfy.models;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Services.ApiJobfy.Services
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FuncionarioService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Recrutador>> GetFuncionariosAsync(int page, int pageSize)
        {
            return await _dbContext.Recrutadores
                .Skip((page - 1) * pageSize)  
                .Take(pageSize)              
                .ToListAsync();
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
    }
}