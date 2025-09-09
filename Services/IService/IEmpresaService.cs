using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IEmpresaService
    {
        Task<Empresa> AddEmpresaAsync(Empresa empresa);
        Task<IEnumerable<Empresa>> GetEmpresasAsync(int page, int pageSize);
        Task<Empresa?> GetEmpresaByIdAsync(Guid id);
        Task<bool> UpdateEmpresaAsync(Empresa empresa); 
    }
}
