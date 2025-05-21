using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace ApiJobfy.Controllers
{
    [ApiController]
    [Route("api/candidato")]
    public class CandidatoController : ControllerBase
    {
        private readonly ICandidatoService _candidatoService;

        public CandidatoController(ICandidatoService candidatoService)
        {
            _candidatoService = candidatoService;
        }

        [HttpGet("getCandidatos")]
        public async Task<IActionResult> GetCandidatos(int page = 1, int pageSize = 10)
        {
            var candidatos = await _candidatoService.GetCandidatosAsync(page, pageSize);
            return Ok(candidatos);
        }

        [HttpGet("getCandidatoById/{id}")]
        public async Task<IActionResult> GetCandidatoById(int id)
        {
            var candidato = await _candidatoService.GetCandidatoByIdAsync(id);
            if (candidato == null)
                return NotFound();
            return Ok(candidato);
        }

        [HttpPut("updateCandidato")]
        public async Task<IActionResult> UpdateCandidato([FromBody] Candidato candidato)
        {
            if (candidato == null)
                return BadRequest("Candidato não pode ser nulo.");

            var resultado = await _candidatoService.UpdateCandidatoAsync(candidato);
            if (!resultado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("deleteCandidato/{id}")]
        public async Task<IActionResult> DeleteCandidato(int id)
        {
            var resultado = await _candidatoService.SoftDeleteCandidatoAsync(id);
            if (!resultado)
                return NotFound();

            return NoContent();
        }
    }
}
