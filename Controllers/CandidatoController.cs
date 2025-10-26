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
        public async Task<IActionResult> GetCandidatoById(Guid id)
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
        /// <summary>
        /// Criar ou atualizar currículo de um candidato
        /// </summary>
        [HttpPost("createCurriculo/{candidatoId}")]
        public IActionResult CriarOuAtualizar(Guid candidatoId, [FromBody] Curriculo curriculo)
        {
            if (string.IsNullOrWhiteSpace(curriculo.Tecnologias) ||
                string.IsNullOrWhiteSpace(curriculo.CompetenciasTecnicas) ||
                string.IsNullOrWhiteSpace(curriculo.Idiomas))
            {
                return BadRequest("Os campos Tecnologias, CompetenciasTecnicas e Idiomas são obrigatórios.");
            }

            var resultado = _candidatoService.CriarOuAtualizar(candidatoId, curriculo);
            return Ok(resultado);
        }

        /// <summary>
        /// Buscar currículo de um candidato
        /// </summary>
        [HttpGet("getCurriculo/{candidatoId}")]
        public IActionResult GetByCandidato(Guid candidatoId)
        {
            var curriculo = _candidatoService.BuscarPorCandidato(candidatoId);
            if (curriculo == null)
                return NotFound("Currículo não encontrado para este candidato.");

            return Ok(curriculo);
        }

        /// <summary>
        /// Deletar currículo de um candidato
        /// </summary>
        [HttpDelete("deleteCurriculo/{candidatoId}")]
        public IActionResult Deletar(Guid candidatoId)
        {
            var sucesso = _candidatoService.Deletar(candidatoId);
            if (!sucesso)
                return NotFound("Currículo não encontrado para exclusão.");

            return NoContent();
        }

    }
}
