using ApiJobfy.models;
using ApiJobfy.models.DTOs;

namespace ApiJobfy.Services
{
    public interface IAuthService
    {
        Task<Candidato> RegisterCandidatoAsync(RegisterCandidatoDto dto);
        Task<string?> LoginAsync(string email, string senha);
    }
}
