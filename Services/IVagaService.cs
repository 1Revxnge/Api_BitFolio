using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.models;

namespace ApiJobfy.Services
{
    public interface IVagaService
    {
        Task<IEnumerable<Vagas>> GetVagasAsync(int page, int pageSize);
        Task<IEnumerable<Vagas>> GetVagasByEmpresaIdAsync(int Empresa);
        Task<Vagas?> GetVagaByIdAsync(int id);
        Task<Vagas> AddVagaAsync(Vagas vaga);
        Task<bool> UpdateVagaAsync(Vagas vaga);
        Task<bool> DeleteVagaAsync(int id);
 
    }
}
