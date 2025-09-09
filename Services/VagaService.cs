using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.EntityFrameworkCore;       

namespace ApiJobfy.Services
{
    public class VagaService : IVagaService
    {
        private readonly AppDbContext _dbContext;

        public VagaService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Vaga>> GetVagasAsync(int page, int pageSize)
        {
            return await _dbContext.Vagas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vaga>> GetVagasByEmpresaIdAsync(Guid Empresas)
        {
            return await _dbContext.Vagas
                .Where(v => v.EmpresaId == Empresas)
                .ToListAsync();
        }

        public async Task<Vaga?> GetVagaByIdAsync(Guid id)
        {
            return await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);
        }

        public async Task<Vaga> AddVagaAsync(Vaga vaga)
        {
            _dbContext.Vagas.Add(vaga);
            await _dbContext.SaveChangesAsync();
            return vaga;
        }

        public async Task<bool> UpdateVagaAsync(Vaga vaga)
        {
            var existingVaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == vaga.VagaId);

            if (existingVaga == null)
                return false;

            existingVaga.Titulo = vaga.Titulo;
            existingVaga.Descricao = vaga.Descricao;
            existingVaga.Empresa = vaga.Empresa;
            
            
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVagaAsync(Guid id)
        {
            var vaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);

            if (vaga == null)
                return false;

            _dbContext.Vagas.Remove(vaga);
            await _dbContext.SaveChangesAsync();
            return true;
        }


       
    }
}
