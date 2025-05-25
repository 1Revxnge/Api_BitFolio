using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using ApiJobfy.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiJobfy.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register/candidato")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCandidato([FromForm] RegisterCandidatoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _userService.ExistsByEmailAsync(dto.Email);
            if (exists)
                return Conflict("Email já cadastrado");

            var user = await _authService.RegisterCandidatoAsync(dto);

            return Ok(new { user.Id, user.Nome, user.Email });
        }
        [HttpPost("register/funcionario")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterFuncionario([FromBody] RegisterFuncionarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _userService.ExistsByEmailAsync(dto.Email);
            if (exists)
                return Conflict("Email já cadastrado");

            var user = await _authService.RegisterFuncionarioAsync(dto);

            return Ok(new { user.Id, user.Nome, user.Email });
        }

        [HttpPost("register/admin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _userService.ExistsByEmailAsync(dto.Email);
            if (exists)
                return Conflict("Email já cadastrado");

            var user = await _authService.RegisterAdministradorAsync(dto);

            return Ok(new { user.Id, user.Nome, user.Email });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto.Email, dto.Senha, dto.Tipo);
                return Ok(new { Token = token });
            }
            catch (InvalidOperationException ex)
            {
                var mensagem = ex.Message;
                int? tentativasRestantes = null;

                var partes = mensagem.Split("Tentativas restantes:");
                if (partes.Length == 2 && int.TryParse(partes[1].Trim(), out var restantes))
                {
                    tentativasRestantes = restantes;
                    mensagem = partes[0].Trim();
                }

                return Unauthorized(new
                {
                    success = false,
                    mensagem,
                    tentativasRestantes
                });
            }
        }

        [HttpPost("recuperar-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> EnviarCodigoRecuperacao([FromBody] string email)
        {
            try
            {
                await _authService.EnviarTokenRecuperacaoAsync(email);
                return Ok(new { mensagem = "Código de recuperação enviado para o e-mail informado." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = $"Erro ao enviar código: {ex.Message}" });
            }
        }
        [HttpPost("redefinir-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDto dto)
        {
            try
            {
                await _authService.RedefinirSenhaAsync(dto.Email, dto.Codigo, dto.NovaSenha);
                return Ok(new { mensagem = "Senha redefinida com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
