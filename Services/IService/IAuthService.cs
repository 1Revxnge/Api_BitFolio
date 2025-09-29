using ApiJobfy.models;
using ApiJobfy.models.DTOs;

namespace ApiJobfy.Services.IService
{
    public interface IAuthService
    {
        Task<Candidato> RegisterCandidatoAsync(RegisterCandidatoDto dto);
        Task<Administrador> RegisterAdministradorAsync(RegisterAdminDto dto);
        Task<Recrutador> RegisterFuncionarioAsync(RegisterFuncionarioDto dto);
        Task EnviarTokenRecuperacaoAsync(string email);
        Task RedefinirSenhaAsync(string email, string token, string novaSenha);
        Task<LoginResult> LoginAsync(string email, string senha, string tipo);
        Task<string?> ValidarToken2FAAsync(string email, string codigo);
        Task ConfirmarEmailAsync(string token);
    }
}
