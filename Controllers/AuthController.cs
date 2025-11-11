using ApiJobfy.models;
using ApiJobfy.models.DTOs;
using ApiJobfy.Services;
using ApiJobfy.Services.IService;
using BitFolio.models.DTOs;
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
        private readonly IEmpresaService _empresaService;
        public AuthController(IAuthService authService, IUserService userService, IEmpresaService empresaService)
        {
            _authService = authService;
            _userService = userService;
            _empresaService = empresaService;
        }

        [HttpPost("register/candidato")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCandidato([FromBody] RegisterCandidatoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _userService.ExistsByEmailAsync(dto.Email);
            if (exists)
                return Conflict("Email já cadastrado");

            var user = await _authService.RegisterCandidatoAsync(dto);

            return Ok(new { user.CandidatoId, user.Nome, user.Email });
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

            return Ok(new { user.RecrutadorId, user.Nome, user.Email });
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

            return Ok(new { user.AdminId, user.Nome, user.Email });
        }
        [HttpPost("register/empresa")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterEmpresa([FromBody] RegisterEmpresaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _userService.ExistsByEmailAsync(dto.Email);
            if (exists)
                return Conflict("Email já cadastrado");

            var empresaExists = await _empresaService.ExistsByCnpjAsync(dto.CNPJ);
            if (empresaExists)
                return Conflict("CNPJ já cadastrado");

            var empresa = await _authService.RegisterEmpresaAsync(dto);

            return Ok(new
            {
                empresa.EmpresaId,
                empresa.Nome,
                empresa.CNPJ,
                empresa.Email,
                empresa.Ativo
            });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto.Email, dto.Senha, dto.Tipo);
                if (result.DoisFatoresNecessario)
                {
                    return Ok(new { DoisFatoresNecessario = true });
                }

                return Ok(new
                {
                    token = result.Token,              // string JWT
                    doisFatoresNecessario = false
                });
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
        [HttpPost("validar-2fa")]
        [AllowAnonymous]
        public async Task<IActionResult> Validar2FA([FromBody] TokenTemporario dto)
        {
            try
            {
                // Chama o serviço para validar o código 2FA e obter o JWT
                var token = await _authService.ValidarToken2FAAsync(dto.Email, dto.Codigo);

                if (string.IsNullOrEmpty(token))
                {
                    // Retorna Unauthorized se o código for inválido
                    return Unauthorized(new { sucesso = false, mensagem = "Código de verificação inválido ou expirado." });
                }

                // Retorna o token JWT se a validação for bem-sucedida
                return Ok(new { sucesso = true, token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = ex.Message });
            }
        }
        [HttpGet("confirmar-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarEmail([FromQuery] string token)
        {
            try
            {
                await _authService.ConfirmarEmailAsync(token);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
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
