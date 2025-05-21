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

            return CreatedAtAction(nameof(RegisterCandidato), new { id = user.Id }, new { user.Id, user.Nome, user.Email });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Senha);

            if (token == null)
                return Unauthorized("Credenciais inválidas");

            return Ok(new { Token = token });
        }
    }
}
