using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface ICandidatoService
    {
        Task<IEnumerable<Candidato>> GetCandidatosAsync(int page, int pageSize);
        Task<Candidato?> GetCandidatoByIdAsync(Guid id);
        Task<bool> UpdateCandidatoAsync(Candidato candidato);
        Curriculo CriarOuAtualizar(Guid candidatoId, Curriculo curriculo);
        Curriculo? BuscarPorCandidato(Guid candidatoId);
        bool Deletar(Guid candidatoId);
    }
}
