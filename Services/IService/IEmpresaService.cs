using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IEmpresaService
    {
        [ExcludeFromCodeCoverage]

        Task<Empresa> AddEmpresaAsync(Empresa empresa);
        Task<IEnumerable<Empresa>> GetEmpresasAsync(int page, int pageSize);
        Task<Empresa?> GetEmpresaByIdAsync(Guid id);
        Task<bool> UpdateEmpresaAsync(Empresa empresa);
        Task<bool> ExistsByCnpjAsync(string cnpj);
        Task<IEnumerable<Empresa>> GetTodasEmpresasAsync(int page, int take);
        Task<Empresa?> AprovarEmpresaAsync(Guid empresaId);
        Task<bool> ReprovarEmpresaAsync(Guid empresaId);
        Task<int> GetTotalEmpresasAsync();

    }
}
