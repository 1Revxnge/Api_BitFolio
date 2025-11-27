using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ApiJobfy.models;
using BitFolio.models;
using BitFolio.models.DTOs;

namespace ApiJobfy.Services.IService
{
    public interface IVagaService
    {
        [ExcludeFromCodeCoverage]
        Task<IEnumerable<Vaga>> GetVagasAsync(int page, int pageSize);
        Task<int> GetTotalVagasAsync();
        Task<IEnumerable<Vaga>> GetVagasByEmpresaIdAsync(Guid empresaId, int page, int pageSize);
        Task<int> GetTotalVagasByEmpresaAsync(Guid empresaId);
        Task<Vaga?> GetVagaByIdAsync(Guid id);
        Task<Vaga> AddVagaAsync(Vaga vaga);
        Task<Vaga?> UpdateVagaAsync(Vaga vaga);
        Task<bool> DeleteVagaAsync(Guid id);
        Task<bool> ToggleFavoritoAsync(Guid candidatoId, Guid vagaId);
        Task<IEnumerable<Vaga>> GetFavoritosByCandidatoAsync(Guid candidatoId);
        Task<(IEnumerable<VagaDTO> Vagas, int TotalCount)> BuscarPorFiltros(FiltroVagaDTO filtro, Guid candidatoId);
        ResultadoCandidaturaDTO Candidatar(Guid candidatoId, Guid vagaId);
        Task<object> AtualizarStatusAsync(AtualizarStatusRequest request);
        Task<IEnumerable<HistoricoCandidatura>> GetHistoricoAsync(Guid candidatoId);
        Task<IEnumerable<CandidatoVaga>> GetCandidatosDaVagaAsync(Guid vagaId, int? status, string? search);
        Task<CandidatoStatusCountDto> GetCandidatosCountsAsync(Guid vagaId);
        
    }
}