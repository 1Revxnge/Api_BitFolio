using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IAdministradorService
    {
        Task<IEnumerable<Administrador>> GetAdministradoresAsync(int page, int pageSize);
        Task<Administrador?> GetAdministradorByIdAsync(Guid id);
        Task<bool> UpdateAdministradorAsync(Administrador administrador);
        Task<Administrador> AddAdministradorAsync(Administrador administrador);
        Task<bool> DeleteAdministradorAsync(Guid id); 
    }
}
