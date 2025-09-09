using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface ICandidatoService
    {
        Task<IEnumerable<Candidato>> GetCandidatosAsync(int page, int pageSize);
        Task<Candidato?> GetCandidatoByIdAsync(Guid id);
        Task<bool> UpdateCandidatoAsync(Candidato candidato);
    }
}
