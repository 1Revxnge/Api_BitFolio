using Microsoft.AspNetCore.Mvc;
using ApiJobfy.Services;
using ApiJobfy.models;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
        [Route("api/funcionario")]
        public class FuncionarioController : ControllerBase
        {
            private readonly IFuncionarioService _funcionarioService;
            public FuncionarioController(IFuncionarioService funcionarioService)
            {
                _funcionarioService = funcionarioService;
            }

        [HttpGet("getFunc")]
        public async Task<IActionResult> GetFunc(int page = 1, int take = 10)
        {
            // total de funcionários ativos
            var qtd = await _funcionarioService.GetTotalFuncionariosAsync();

            var pages = take != 0 ? (qtd / take) : 1;
            int skip = take * (page - 1);

            if (take != 0 && (qtd % take) != 0)
                pages += 1;

            // lista paginada
            var funcionarios = await _funcionarioService.GetFuncionariosAsync(page, take);

            var response = new ObjectResult(funcionarios)
            {
                StatusCode = StatusCodes.Status200OK
            };

            Response.Headers.Append("Access-Control-Expose-Headers", "pages, qtd, range");
            Response.Headers.Append("pages", pages.ToString());
            Response.Headers.Append("qtd", qtd.ToString());

            if (take != 0)
            {
                var rangeInicio = skip + 1;
                var rangeFim = ((skip + take) - qtd) >= 0 ? qtd : (skip + take);
                Response.Headers.Append("range", $"{rangeInicio}-{rangeFim}");
            }
            else
            {
                Response.Headers.Append("range", $"1-{qtd}");
            }

            return response;
        }


        [HttpGet("getFuncById/{id}")]
            public async Task<IActionResult> GetFuncById(Guid id)
            {
                var funcionario = await _funcionarioService.GetFuncionarioByIdAsync(id);
                if (funcionario == null)
                    return NotFound();

                return Ok(funcionario);
            }

            [HttpPut("updateFuncionario")]
            public async Task<IActionResult> UpdateFuncionario([FromBody] Recrutador funcionario)
            {
                if (funcionario == null)
                    return BadRequest("Funcionário não pode ser nulo.");

                var resultado = await _funcionarioService.UpdateFuncionarioAsync(funcionario);
                if (!resultado)
                    return NotFound();

                return NoContent();
            }
       
            [HttpDelete("deleteFuncionario/{id}")]
            public async Task<IActionResult> DeleteFuncionario(Guid id)
            {
                var resultado = await _funcionarioService.DeleteFuncionarioAsync(id);
                if (!resultado)
                    return NotFound();

                return NoContent();
            }
        [HttpGet("getLogsRecrutador/{recrutadorId}")]
        public async Task<IActionResult> GetLogsRecrutador(Guid recrutadorId, int page = 1, int pageSize = 10)
        {
            // 1 — total de logs
            var qtd = await _funcionarioService.GetTotalLogsRecrutadorAsync(recrutadorId);

            // cálculo de páginas
            var pages = pageSize != 0 ? (qtd / pageSize) : 1;
            int skip = pageSize * (page - 1);

            if (pageSize != 0 && (qtd % pageSize) != 0)
                pages += 1;

            // 2 — dados paginados
            var logs = await _funcionarioService.GetLogsRecrutadorAsync(recrutadorId, page, pageSize);

            // 3 — resposta padrão
            var response = new ObjectResult(logs)
            {
                StatusCode = StatusCodes.Status200OK
            };

            // 4 — Headers
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


    }
}
