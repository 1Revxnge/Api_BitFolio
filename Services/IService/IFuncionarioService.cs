using ApiJobfy.models;

namespace ApiJobfy.Services
{
    namespace ApiJobfy.Services
    {
        public interface IFuncionarioService
        {
            Task<IEnumerable<Recrutador>> GetFuncionariosAsync(int page, int pageSize);
            Task<Recrutador?> GetFuncionarioByIdAsync(Guid id);
            Task<bool> UpdateFuncionarioAsync(Recrutador funcionario);
            Task<bool> DeleteFuncionarioAsync(Guid id);
        }
    }
}
