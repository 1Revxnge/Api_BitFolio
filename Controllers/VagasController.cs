using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using BitFolio.models;

namespace ApiJobfy.Controllers
{
    [ApiController]
    [Route("api/vagas")]
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
        public async Task<IActionResult> GetVagasByNegocio(Guid empresaId)
        {
            var vagas = await _vagaService.GetVagasByEmpresaIdAsync(empresaId);
            return Ok(vagas);
        }

        [HttpGet("getVagasById/{id}")]
        public async Task<IActionResult> GetVagaById(Guid id)
        {
            var vagas = await _vagaService.GetVagaByIdAsync(id);
            if (vagas == null)
                return NotFound();
            return Ok(vagas);
        }

        [HttpPost("criarVaga")]
        public async Task<IActionResult> CriarVaga([FromBody] Vaga vaga)
        {
            if (vaga == null)
                return BadRequest("Vaga não pode ser nula.");

            var vagaCriada = await _vagaService.AddVagaAsync(vaga);
            return CreatedAtAction(nameof(GetVagaById), new { id = vagaCriada.VagaId }, vagaCriada);
        }

        [HttpPut("updateVaga")]
        public async Task<IActionResult> UpdateVaga([FromBody] Vaga vaga)
        {
            if (vaga == null)
                return BadRequest("Vaga não pode ser nula.");

            var vagaAtualizada = await _vagaService.UpdateVagaAsync(vaga);
            if (vagaAtualizada == null)
                return NotFound();

            return Ok(vagaAtualizada); 
        }
        [HttpDelete("deleteVaga/{id}")]
        public async Task<IActionResult> DeleteVaga(Guid id)
        {
            var deletado = await _vagaService.DeleteVagaAsync(id);
            if (!deletado)
                return NotFound();

            return NoContent();
        }

        [HttpPost("toggleFavorito")]
        public async Task<IActionResult> ToggleFavorito([FromBody] ToggleFavorito dto)
        {
            var result = await _vagaService.ToggleFavoritoAsync(dto.CandidatoId, dto.VagaId);
            if (!result)
                return BadRequest("Erro ao favoritar/desfavoritar vaga.");
            return Ok("Operação realizada com sucesso.");
        }

        [HttpGet("favoritos/{candidatoId}")]
        public async Task<IActionResult> GetFavoritos(Guid candidatoId)
        {
            var favoritos = await _vagaService.GetFavoritosByCandidatoAsync(candidatoId);
            return Ok(favoritos);
        }

        [HttpGet("buscar")]
        public IActionResult Buscar([FromQuery] string palavrasChave, [FromQuery] int page = 1, [FromQuery] int take = 10)
        {
            // Total de registros
            var query = _vagaService.BuscarPorPalavrasChave(palavrasChave); 
            var qtd = query.Count();

            // Total de páginas
            var pages = take != 0 ? (qtd / take) : 1;
            if (take != 0 && (qtd % take) != 0)
                pages += 1;

            // Skip/Take
            int skip = take * (page - 1);

            IEnumerable<Vaga> vagas;
            if (take != 0)
                vagas = query.OrderBy(v => v.Titulo).Skip(skip).Take(take).ToList();
            else
                vagas = query.OrderBy(v => v.Titulo).ToList();

            // Criar response
            var response = new ObjectResult(vagas)
            {
                StatusCode = StatusCodes.Status200OK
            };

            // Adicionar headers de paginação
            Response.Headers.Append("Access-Control-Expose-Headers", "pages, qtd, range");
            Response.Headers.Append("pages", pages.ToString());
            Response.Headers.Append("qtd", qtd.ToString());

            if (take != 0)
                Response.Headers.Append("range", ((skip + take) - qtd) >= 0
                    ? $"{skip + 1}-{qtd}"
                    : $"{skip + 1}-{skip + take}");
            else
                Response.Headers.Append("range", $"1-{qtd}");

            return response;
        }

    }
}
