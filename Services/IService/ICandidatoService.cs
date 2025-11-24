using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services.IService
{
    public interface ICandidatoService
    {
        [ExcludeFromCodeCoverage]

        Task<IEnumerable<Candidato>> GetCandidatosAsync(int page, int pageSize);
        Task<Candidato?> GetCandidatoByIdAsync(Guid id);
        Task<bool> UpdateCandidatoAsync(Candidato candidato);
        Curriculo CriarOuAtualizar(Guid candidatoId, Curriculo curriculo);
        Curriculo? BuscarPorCandidato(Guid candidatoId);
        bool Deletar(Guid candidatoId);
        Task<int> GetTotalLogsCandidatoAsync(Guid candidatoId);
        Task<List<LogCandidato>> GetLogsCandidatoAsync(Guid candidatoId, int page, int pageSize);
    }
}
