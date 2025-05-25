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
                .Where(e => e.Ativo) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Empresas?> GetEmpresaByIdAsync(int id)
        {
            return await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id == id && e.Ativo);
        }

        public async Task<Empresas> AddEmpresaAsync(Empresas empresa)
        {
            empresa.Ativo = true;
            empresa.DtCadastro = DateTime.UtcNow;
            empresa.DtAprovacao = DateTime.UtcNow;
            _dbContext.Empresas.Add(empresa);
            await _dbContext.SaveChangesAsync();
            return empresa;
        }

        public async Task<bool> UpdateEmpresaAsync(Empresas empresa)
        {
            var existingEmpresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.Id == empresa.Id);

            if (existingEmpresa == null)
                return false;

            existingEmpresa.Nome = empresa.Nome;
            existingEmpresa.Email = empresa.Email;
            existingEmpresa.Cnpj = empresa.Cnpj;
            existingEmpresa.Descricao = empresa.Descricao;
            existingEmpresa.LogoUrl = empresa.LogoUrl;
            existingEmpresa.DtAprovacao = empresa.DtAprovacao;
            existingEmpresa.EnderecoId = empresa.EnderecoId;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteEmpresaAsync(int id)
        {
            var empresa = await _dbContext.Empresas.FirstOrDefaultAsync(e => e.Id == id);
            if (empresa == null)
                return false;

            empresa.Ativo = false;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
