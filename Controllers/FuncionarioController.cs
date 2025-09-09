using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiJobfy.Services;
using ApiJobfy.models;
using ApiJobfy.Services.ApiJobfy.Services;

namespace ApiJobfy.Controllers
{

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
            public async Task<IActionResult> GetFunc(int page = 1, int pageSize = 10)
            {
                var funcionarios = await _funcionarioService.GetFuncionariosAsync(page, pageSize);
                return Ok(funcionarios);
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


        }
    }
