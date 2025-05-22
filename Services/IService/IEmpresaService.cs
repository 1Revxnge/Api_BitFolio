using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IEmpresaService
    {
        Task<Empresas> AddEmpresaAsync(Empresas empresa);
        Task<IEnumerable<Empresas>> GetEmpresasAsync(int page, int pageSize);
        Task<Empresas?> GetEmpresaByIdAsync(int id);
        Task<bool> UpdateEmpresa(Empresas empresas);
        Task<bool> SoftDeleteEmpresaAsync(int id);
    }
}
