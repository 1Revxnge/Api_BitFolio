using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Services
{
    public class AdministradorService : IAdministradorService
    {
        private readonly AppDbContext _dbContext;

        public AdministradorService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Administrador>> GetAdministradoresAsync(int page, int pageSize)
        {
            return await _dbContext.Administradores
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Administrador?> GetAdministradorByIdAsync(Guid id)
        {
            return await _dbContext.Administradores
                .FirstOrDefaultAsync(a => a.AdminId == id);
        }

        public async Task<bool> UpdateAdministradorAsync(Administrador administrador)
        {
            var existing = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.AdminId == administrador.AdminId);

            if (existing == null)
                return false;

            existing.Nome = administrador.Nome;
            existing.Email = administrador.Email;
            existing.Telefone = administrador.Telefone;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Administrador> AddAdministradorAsync(Administrador administrador)
        {
            _dbContext.Administradores.Add(administrador);
            await _dbContext.SaveChangesAsync();
            return administrador;
        }

        public async Task<bool> DeleteAdministradorAsync(Guid id)
        {
            var administrador = await _dbContext.Administradores.FirstOrDefaultAsync(a => a.AdminId == id);
            if (administrador == null)
                return false;

            _dbContext.Administradores.Remove(administrador);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}