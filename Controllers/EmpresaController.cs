using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using System.Diagnostics.CodeAnalysis;
using BitFolio.models;

namespace ApiJobfy.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
        [Route("api/empresa")]
        public class EmpresaController : ControllerBase
        {
            private readonly IEmpresaService _empresaService;
            private readonly IEnderecoService _enderecoService;

            public EmpresaController(IEmpresaService empresaService, IEnderecoService enderecoService)
            {
                _empresaService = empresaService;
                _enderecoService = enderecoService;
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
        [HttpPost("alterarEndereco/{funcionarioId}")]
        public async Task<IActionResult> SolicitarAlteracaoEndereco(
       [FromBody] SolicitacaoEndereco dto, Guid funcionarioId)
        {
            var result = await _enderecoService.SolicitarAlteracaoEnderecoAsync(dto, funcionarioId);

            if (!result)
                return BadRequest("Erro ao solicitar alteração de endereço.");

            return Ok(new { message = "Solicitação enviada com sucesso!" });
        }
        [HttpGet("solicitacoes/aprovar/{id}")]
        public async Task<IActionResult> AprovarSolicitacaoEndereco(Guid id)
        {
            var resultado = await _empresaService.AprovarSolicitacaoEnderecoAsync(id);

            if (!resultado)
                return BadRequest("Solicitação não encontrada ou já aprovada.");

            return Ok("Endereço atualizado com sucesso!");
        }

        [HttpPost("alterarDados/{funcionarioId}")]
        public async Task<IActionResult> SolicitarAlteracaoEmpresa(
    [FromBody] SolicitacaoEmpresa dto, Guid funcionarioId)
        {
            var result = await _empresaService.SolicitarAlteracaoEmpresaAsync(dto, funcionarioId);

            if (!result)
                return BadRequest("Erro ao solicitar alteração dos dados da empresa.");

            return Ok("Solicitação enviada com sucesso!");
        }

        [HttpGet("solicitacoes/aprovarDados/{id}")]
        public async Task<IActionResult> AprovarSolicitacaoEmpresa(Guid id)
        {
            var resultado = await _empresaService.AprovarSolicitacaoEmpresaAsync(id);

            if (!resultado)
                return BadRequest("Solicitação não encontrada ou já aprovada.");

            return Ok("Dados da empresa atualizados com sucesso!");
        }


    }
}
