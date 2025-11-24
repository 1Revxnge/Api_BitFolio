using ApiJobfy.models;
using ApiJobfy.Services;
using ApiJobfy.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("api/administrador")]
    public class AdministradorController : ControllerBase
    {
        private readonly IAdministradorService _administradorService;
        private readonly IEmpresaService _empresaService;

        public AdministradorController(IAdministradorService administradorService, IEmpresaService empresaService)
        {
            _administradorService = administradorService;
            _empresaService = empresaService;

        }

        [HttpGet("getAdministradores")]
        public async Task<IActionResult> GetAdministradores(int page = 1, int take = 10)
        {
            var administradores = await _administradorService.GetAdministradoresAsync(page, take);
            return Ok(administradores);
        }

        [HttpGet("getAdministradorById/{id}")]
        public async Task<IActionResult> GetAdministradorById(Guid id)
        {
            var administrador = await _administradorService.GetAdministradorByIdAsync(id);
            if (administrador == null)
                return NotFound();

            return Ok(administrador);
        }

        [HttpPut("updateAdministrador")]
        public async Task<IActionResult> UpdateAdministrador([FromBody] Administrador administrador)
        {
            if (administrador == null)
                return BadRequest("Administrador não pode ser nulo.");

            var resultado = await _administradorService.UpdateAdministradorAsync(administrador);
            if (!resultado)
                return NotFound();

            return NoContent();
        }

   

        [HttpDelete("deleteAdministrador/{id}")]
        public async Task<IActionResult> DeleteAdministrador(Guid id)
        {
            var resultado = await _administradorService.DeleteAdministradorAsync(id);
            if (!resultado)
                return NotFound();

            return NoContent();
        }

        [HttpPut("empresa/aprovar/{id}")]
        public async Task<IActionResult> AprovarEmpresa(Guid id)
        {
            var empresa = await _empresaService.AprovarEmpresaAsync(id);

            if (empresa == null)
                return NotFound();

            return Ok(new { Message = "Empresa aprovada com sucesso", empresa });
        }

        [HttpDelete("empresa/reprovar/{id}")]
        public async Task<IActionResult> ReprovarEmpresa(Guid id)
        {
            var sucesso = await _empresaService.ReprovarEmpresaAsync(id);

            if (!sucesso)
                return NotFound();

            return Ok(new { Message = "Empresa reprovada e removida com sucesso" });
        }

    }
}
