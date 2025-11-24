using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using System.Diagnostics.CodeAnalysis;

    namespace ApiJobfy.Services
    {
        public interface IFuncionarioService
        {
        [ExcludeFromCodeCoverage]

        Task<IEnumerable<Recrutador>> GetFuncionariosAsync(int page, int pageSize);
            Task<Recrutador?> GetFuncionarioByIdAsync(Guid id);
            Task<bool> UpdateFuncionarioAsync(Recrutador funcionario);
            Task<bool> DeleteFuncionarioAsync(Guid id);
            Task<int> GetTotalLogsRecrutadorAsync(Guid recrutadorId);
            Task<List<LogRecrutador>> GetLogsRecrutadorAsync(Guid recrutadorId, int page, int pageSize);
    }
    }

