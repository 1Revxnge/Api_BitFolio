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

            var recrutadores = await _dbContext.Recrutadores
                .Where(r => r.EmpresaId == empresaId)
                .ToListAsync();

            if (recrutadores.Any())
            {
                _dbContext.Recrutadores.RemoveRange(recrutadores);
            }

            _dbContext.Empresas.Remove(empresa);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalEmpresasAsync()
        {
            return await _dbContext.Empresas.CountAsync();
        }
    }
}
