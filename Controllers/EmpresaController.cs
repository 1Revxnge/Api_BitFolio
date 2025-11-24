using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
        [Route("api/empresa")]
        public class EmpresaController : ControllerBase
        {
            private readonly IEmpresaService _empresaService;

            public EmpresaController(IEmpresaService empresaService)
            {
                _empresaService = empresaService;
            }

            [HttpGet("getEmpresas")]
            public async Task<IActionResult> GetEmpresas(int page = 1, int pageSize = 10)
            {
                var empresas = await _empresaService.GetEmpresasAsync(page, pageSize);
                return Ok(empresas);
            }

            [HttpGet("getEmpresasById/{id}")]
            public async Task<IActionResult> GetEmpresasById(Guid id)
            {
                var empresas = await _empresaService.GetEmpresaByIdAsync(id);
                if (empresas == null)
                    return NotFound();
                return Ok(empresas);
            }

            [HttpPost("createEmp")]
            public async Task<IActionResult> CreateEmp([FromBody] Empresa empresas)
            {
                if (empresas == null)
                    return BadRequest("Empresa não pode ser nula.");

                var criado = await _empresaService.AddEmpresaAsync(empresas);
                return CreatedAtAction(nameof(GetEmpresasById), new { id = criado.EmpresaId }, criado);
            }

            [HttpPut("updateEmp")]
            public async Task<IActionResult> UpdateEmp([FromBody] Empresa empresas)
            {
                if (empresas == null)
                    return BadRequest("Empresa não pode ser nula.");

                var resultado = await _empresaService.UpdateEmpresaAsync(empresas);
                if (!resultado)
                    return NotFound();

                return NoContent();
            }

        [HttpGet("GetAllEmpresas")]
        public async Task<IActionResult> GetAllEmpresas(int page = 1, int take = 10)
        {
            // Obtém o total de empresas
            var qtd = await _empresaService.GetTotalEmpresasAsync();

            // Cálculo de páginas
            var pages = take != 0 ? (qtd / take) : 1;
            int skip = take * (page - 1);

            if (take != 0 && (qtd % take) != 0)
                pages += 1;

            // Aplica paginação
            var empresas = await _empresaService.GetTodasEmpresasAsync(page, take);

            // Monta resposta com headers
            var response = new ObjectResult(empresas)
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

    }
}
