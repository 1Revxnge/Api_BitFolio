using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using BitFolio.models.DTOs;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services.IService
{
    public interface IAuthService
    {
        [ExcludeFromCodeCoverage]

        Task<Candidato> RegisterCandidatoAsync(RegisterCandidatoDto dto);
        Task<Administrador> RegisterAdministradorAsync(RegisterAdminDto dto);
        Task<Recrutador> RegisterFuncionarioAsync(RegisterFuncionarioDto dto);
        Task<Empresa> RegisterEmpresaAsync(RegisterEmpresaDTO dto);
        Task EnviarTokenRecuperacaoAsync(string email);
        Task RedefinirSenhaAsync(string email, string token, string novaSenha);
        Task<LoginResult> LoginAsync(string email, string senha, string tipo);
        Task<string?> ValidarToken2FAAsync(string email, string codigo);
        Task ConfirmarEmailAsync(string token);
        Task AlterarSenhaAsync(string email, string senhaAtual, string novaSenha, string confirmacao);
    }
}
