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
                .FirstOrDefaultAsync(e => e.EmpresaId == id && e.Ativo);
        }

        public async Task<Empresa> AddEmpresaAsync(Empresa empresa)
        {
            empresa.Ativo = true;
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

       
    }
}
