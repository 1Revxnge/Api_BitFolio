using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using ApiJobfy.models;
using ApiJobfy.Data;
using ApiJobfy.Services.IService;

namespace ApiJobfy.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly AppDbContext _dbContext;

        public EmpresaService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Empresas>> GetEmpresasAsync(int page, int pageSize)
        {
            return await _dbContext.Empresas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Empresas?> GetEmpresaByIdAsync(int id)
        {
            return await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> UpdateEmpresaAsync(Empresas empresas)
        {
            var existingEmpresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id == empresas.Id);

            if (existingEmpresa == null)
                return false;

            existingEmpresa.Nome = empresas.Nome;
            existingEmpresa.Email = empresas.Email;
           
            

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteEmpresaAsync(int id)
        {
            var empresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id == id);

            if (empresa == null)
                return false;

            empresa.Ativo = false; 
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<Empresas> AddEmpresaAsync(Empresas empresas)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Empresas>> IEmpresaService.GetEmpresasAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        Task<Empresas?> IEmpresaService.GetEmpresaByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateEmpresa(Empresas empresas)
        {
            var existingEmpresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id == empresas.Id);
            if (existingEmpresa == null)
                return false;
            existingEmpresa.Nome = empresas.Nome;
            existingEmpresa.Email = empresas.Email;
            // Atualize outros campos conforme necessário
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}
