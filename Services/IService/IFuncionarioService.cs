using ApiJobfy.models;

namespace ApiJobfy.Services
{
    namespace ApiJobfy.Services
    {
        public interface IFuncionarioService
        {
            Task<IEnumerable<Funcionario>> GetFuncionariosAsync(int page, int pageSize);
            Task<Funcionario?> GetFuncionarioByIdAsync(int id);
            Task<bool> UpdateFuncionarioAsync(Funcionario funcionario);
            Task<Funcionario> AddFuncionarioAsync(Funcionario funcionario);
            Task<bool> SoftDeleteFuncionarioAsync(int id);
        }
    }
}
