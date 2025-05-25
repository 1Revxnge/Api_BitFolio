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

        public int Empresa { get; private set; }

        public VagaService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Vagas>> GetVagasAsync(int page, int pageSize)
        {
            return await _dbContext.Vagas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vagas>> GetVagasByEmpresaIdAsync(int Empresas)
        {
            return await _dbContext.Vagas
                .Where(v => Empresa == Empresas)
                .ToListAsync();
        }

        public async Task<Vagas?> GetVagaByIdAsync(int id)
        {
            return await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vagas> AddVagaAsync(Vagas vaga)
        {
            _dbContext.Vagas.Add(vaga);
            await _dbContext.SaveChangesAsync();
            return vaga;
        }

        public async Task<bool> UpdateVagaAsync(Vagas vaga)
        {
            var existingVaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.Id == vaga.Id);

            if (existingVaga == null)
                return false;

            existingVaga.Titulo = vaga.Titulo;
            existingVaga.Descricao = vaga.Descricao;
            existingVaga.Empresa = vaga.Empresa;
            existingVaga.Localizacao = vaga.Localizacao;
            
            
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVagaAsync(int id)
        {
            var vaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vaga == null)
                return false;

            _dbContext.Vagas.Remove(vaga);
            await _dbContext.SaveChangesAsync();
            return true;
        }


       
    }
}
