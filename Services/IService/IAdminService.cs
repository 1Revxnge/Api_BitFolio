using ApiJobfy.models;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services.IService
{

    public interface IAdministradorService
    {
        [ExcludeFromCodeCoverage]

        Task<IEnumerable<Administrador>> GetAdministradoresAsync(int page, int pageSize);
        Task<Administrador?> GetAdministradorByIdAsync(Guid id);
        Task<bool> UpdateAdministradorAsync(Administrador administrador);
        Task<Administrador> AddAdministradorAsync(Administrador administrador);
        Task<bool> DeleteAdministradorAsync(Guid id); 
    }
}
