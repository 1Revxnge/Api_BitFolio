using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IVagaService
    {
        Task<IEnumerable<Vaga>> GetVagasAsync(int page, int pageSize);
        Task<IEnumerable<Vaga>> GetVagasByEmpresaIdAsync(Guid Empresa);
        Task<Vaga?> GetVagaByIdAsync(Guid id);
        Task<Vaga> AddVagaAsync(Vaga vaga);
        Task<Vaga?> UpdateVagaAsync(Vaga vaga);
        Task<bool> DeleteVagaAsync(Guid id);
        Task<bool> ToggleFavoritoAsync(Guid candidatoId, Guid vagaId);
        Task<IEnumerable<Vaga>> GetFavoritosByCandidatoAsync(Guid candidatoId);
    }
}
