using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using BitFolio.models;
using BitFolio.models.DTOs;

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
        public async Task<IActionResult> GetVagas(int page = 1, int pageSize = 5)
        {
            // Obtém o total de vagas
            var qtd = await _vagaService.GetTotalVagasAsync();

            // Cálculo de páginas
            var pages = pageSize != 0 ? (qtd / pageSize) : 1;
            int skip = pageSize * (page - 1);

            if (pageSize != 0 && (qtd % pageSize) != 0)
                pages += 1;

            // Aplica paginação
            var vagas = await _vagaService.GetVagasAsync(page, pageSize);

            // Monta resposta com headers
            var response = new ObjectResult(vagas)
            {
                StatusCode = StatusCodes.Status200OK
            };

            Response.Headers.Append("Access-Control-Expose-Headers", "pages, qtd, range");
            Response.Headers.Append("pages", pages.ToString());
            Response.Headers.Append("qtd", qtd.ToString());

            if (pageSize != 0)
            {
                var rangeInicio = skip + 1;
                var rangeFim = ((skip + pageSize) - qtd) >= 0 ? qtd : (skip + pageSize);
                Response.Headers.Append("range", $"{rangeInicio}-{rangeFim}");
            }
            else
            {
                Response.Headers.Append("range", $"1-{qtd}");
            }

            return response;
        }

        [HttpGet("getVagasByNegocio/{empresaId}")]
        public async Task<IActionResult> GetVagasByNegocio(Guid empresaId, int page = 1, int pageSize = 5)
        {
            // 1) Obtém total de vagas da empresa
            var qtd = await _vagaService.GetTotalVagasByEmpresaAsync(empresaId);

            // Cálculo de páginas
            var pages = pageSize != 0 ? (qtd / pageSize) : 1;
            int skip = pageSize * (page - 1);

            if (pageSize != 0 && (qtd % pageSize) != 0)
                pages += 1;

            // 2) Busca paginada
            var vagas = await _vagaService.GetVagasByEmpresaIdAsync(empresaId, page, pageSize);

            // 3) Monta resposta
            var response = new ObjectResult(vagas)
            {
                StatusCode = StatusCodes.Status200OK
            };

            // 4) Mesmos headers da outra rota
            Response.Headers.Append("Access-Control-Expose-Headers", "pages, qtd, range");
            Response.Headers.Append("pages", pages.ToString());
            Response.Headers.Append("qtd", qtd.ToString());

            if (pageSize != 0)
            {
                var rangeInicio = skip + 1;
                var rangeFim = ((skip + pageSize) - qtd) >= 0 ? qtd : (skip + pageSize);
                Response.Headers.Append("range", $"{rangeInicio}-{rangeFim}");
            }
            else
            {
                Response.Headers.Append("range", $"1-{qtd}");
            }

            return response;
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
            return Ok(new { favoritado = result });
        }

        [HttpGet("favoritos/{candidatoId}")]
        public async Task<IActionResult> GetFavoritos(Guid candidatoId)
        {
            var favoritos = await _vagaService.GetFavoritosByCandidatoAsync(candidatoId);
            return Ok(favoritos);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] FiltroVagaDTO filtro, [FromQuery] Guid candidatoId)
        {
            var (vagasFiltradas, totalCount) = await _vagaService.BuscarPorFiltros(filtro, candidatoId);

            int take = filtro.Take;
            int page = filtro.Page;
            var totalPages = take != 0 ? (int)Math.Ceiling((double)totalCount / take) : 1;
            int skip = take * (page - 1);

            var vagasPaginadas = vagasFiltradas;

            Response.Headers.Append("Access-Control-Expose-Headers", "pages, qtd, range");
            Response.Headers.Append("pages", totalPages.ToString());
            Response.Headers.Append("qtd", totalCount.ToString());
            Response.Headers.Append("range", $"{skip + 1}-{Math.Min(skip + take, totalCount)}");

            return Ok(vagasPaginadas);
        }


        /// <summary>
        /// Candidatar um candidato a uma vaga (envio automático do currículo)
        /// </summary>
        [HttpPost("{candidatoId}/{vagaId}")]
        public IActionResult Candidatar(Guid candidatoId, Guid vagaId)
        {
            var resultado = _vagaService.Candidatar(candidatoId, vagaId);

            if (!resultado.Sucesso)
            {
                // Retorna um código específico que o front pode usar pra abrir o modal
                return BadRequest(new
                {
                    mensagem = resultado.Mensagem,
                    codigo = resultado.Codigo 
                });
            }

            return Ok(new
            {
                mensagem = "Candidatura enviada com sucesso.",
                historicoId = resultado.HistoricoId
            });
        }
        /// <summary>
        /// Atualiza o status da candidatura de um candidato em uma vaga.
        /// </summary>
        [HttpPut("atualizar-status")]
        public async Task<IActionResult> AtualizarStatus([FromBody] AtualizarStatusRequest request)
        {
            try
            {
                var resultado = await _vagaService.AtualizarStatusAsync(request);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
        [HttpGet("historico/{candidatoId}")]
        public async Task<IActionResult> GetHistorico(Guid candidatoId, [FromQuery] int page = 1, [FromQuery] int take = 10)
        {
            // Buscar todo o histórico
            var historico = await _vagaService.GetHistoricoAsync(candidatoId);

            if (historico == null || !historico.Any())
                return NotFound(new { message = "Nenhum histórico encontrado para este candidato." });

            // Total de registros
            int totalCount = historico.Count();

            // Total de páginas
            int totalPages = take != 0 ? (int)Math.Ceiling((double)totalCount / take) : 1;

            // Paginação
            int skip = (page - 1) * take;
            var historicoPaginado = historico.Skip(skip).Take(take);

            // Adiciona headers
            Response.Headers.Append("Access-Control-Expose-Headers", "pages,qtd,range");
            Response.Headers.Append("pages", totalPages.ToString());
            Response.Headers.Append("qtd", totalCount.ToString());
            Response.Headers.Append("range", $"{skip + 1}-{Math.Min(skip + take, totalCount)}");

            return Ok(historicoPaginado);
        }

        [HttpGet("candidatos/{vagaId}")]
        public async Task<IActionResult> GetCandidatosDaVaga(
    Guid vagaId,
    [FromQuery] int? status = null,
    [FromQuery] string? search = null,
    [FromQuery] int page = 1,
    [FromQuery] int take = 10)
        {
            var result = await _vagaService.GetCandidatosDaVagaAsync(vagaId, status, search);

            if (result == null || !result.Any())
                return NotFound(new { message = "Nenhum candidato encontrado para esta vaga." });

            var counts = await _vagaService.GetCandidatosCountsAsync(vagaId);

            Response.Headers.Append("Access-Control-Expose-Headers", "pages,qtd,range,total,emAnalise,entrevista,aprovados,rejeitados");

            Response.Headers.Append("total", counts.Total.ToString());
            Response.Headers.Append("emAnalise", counts.EmAnalise.ToString());
            Response.Headers.Append("revisado", counts.Revisado.ToString());
            Response.Headers.Append("entrevista", counts.Entrevista.ToString());
            Response.Headers.Append("aprovados", counts.Aprovados.ToString());
            Response.Headers.Append("rejeitados", counts.Rejeitados.ToString());

            int totalCount = result.Count();
            int totalPages = take > 0 ? (int)Math.Ceiling((double)totalCount / take) : 1;

            int skip = (page - 1) * take;
            var paginado = result.Skip(skip).Take(take);

            Response.Headers.Append("pages", totalPages.ToString());
            Response.Headers.Append("qtd", totalCount.ToString());
            Response.Headers.Append("range", $"{skip + 1}-{Math.Min(skip + take, totalCount)}");

            return Ok(paginado);
        }
    }
}
