using ApiJobfy.models;
using ApiJobfy.models.DTOs;

namespace ApiJobfy.Services.IService
{
    public interface IAuthService
    {
        Task<Candidato> RegisterCandidatoAsync(RegisterCandidatoDto dto);
        Task<Administrador> RegisterAdministradorAsync(RegisterAdminDto dto);
        Task<Funcionario> RegisterFuncionarioAsync(RegisterFuncionarioDto dto);
        Task EnviarTokenRecuperacaoAsync(string email);
        Task RedefinirSenhaAsync(string email, string token, string novaSenha);
        Task<string?> LoginAsync(string email, string senha, string tipo);
    }
}
