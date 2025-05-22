using Microsoft.AspNetCore.Mvc;
using ApiJobfy.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApiJobfy.models;

namespace ApiJobfy.Controllers
{
    [ApiController]
    [Route("api_jobfy/[controller]")]
    public class VagasController : ControllerBase
    {
        private readonly IVagaService _vagaService;

        public VagasController(IVagaService vagaService)
        {
            _vagaService = vagaService;
        }

        [HttpGet("getVagas")]
        public async Task<IActionResult> GetVagas(int page = 1, int pageSize = 10)
        {
            var vagas = await _vagaService.GetVagasAsync(page, pageSize);
            return Ok(vagas);
        }

        [HttpGet("getVagasByNegocio/{empresaId}")]
        public async Task<IActionResult> GetVagasByNegocio(int empresaId)
        {
            var vagas = await _vagaService.GetVagasByEmpresaIdAsync(empresaId);
            return Ok(vagas);
        }

        [HttpGet("getVagasById/{id}")]
        public async Task<IActionResult> GetVagaById(int id)
        {
            var vagas = await _vagaService.GetVagaByIdAsync(id);
            if (vagas == null)
                return NotFound();
            return Ok(vagas);
        }

        [HttpPost("criarVaga")]
        public async Task<IActionResult> CriarVaga([FromBody] Vagas vaga)
        {
            if (vaga == null)
                return BadRequest("Vaga não pode ser nula.");

            var vagaCriada = await _vagaService.AddVagaAsync(vaga);
            return CreatedAtAction(nameof(GetVagaById), new { id = vagaCriada.Id }, vagaCriada);
        }

        [HttpPut("updateVaga")]
        public async Task<IActionResult> UpdateVaga([FromBody] Vagas vaga)
        {
            if (vaga == null)
                return BadRequest("Vaga não pode ser nula.");

            var atualizado = await _vagaService.UpdateVagaAsync(vaga);
            if (!atualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("deleteVaga/{id}")]
        public async Task<IActionResult> DeleteVaga(int id)
        {
            var deletado = await _vagaService.DeleteVagaAsync(id);
            if (!deletado)
                return NotFound();

            return NoContent();
        }
    }
}
