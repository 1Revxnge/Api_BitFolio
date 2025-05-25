using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiJobfy.models;
using ApiJobfy.Services.IService;

namespace ApiJobfy.Controllers
{
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
            public async Task<IActionResult> GetEmpresasById(int id)
            {
                var empresas = await _empresaService.GetEmpresaByIdAsync(id);
                if (empresas == null)
                    return NotFound();
                return Ok(empresas);
            }

            [HttpPost("createEmp")]
            public async Task<IActionResult> CreateEmp([FromBody] Empresas empresas)
            {
                if (empresas == null)
                    return BadRequest("Empresa não pode ser nula.");

                var criado = await _empresaService.AddEmpresaAsync(empresas);
                return CreatedAtAction(nameof(GetEmpresasById), new { id = criado.Id }, criado);
            }

            [HttpPut("updateEmp")]
            public async Task<IActionResult> UpdateEmp([FromBody] Empresas empresas)
            {
                if (empresas == null)
                    return BadRequest("Empresa não pode ser nula.");

                var resultado = await _empresaService.UpdateEmpresaAsync(empresas);
                if (!resultado)
                    return NotFound();

                return NoContent();
            }

            [HttpDelete("deleteEmp/{id}")]
            public async Task<IActionResult> DeleteEmp(int id)
            {
                var resultado = await _empresaService.SoftDeleteEmpresaAsync(id);
                if (!resultado)
                    return NotFound();

                return NoContent();
            }
    }
}
