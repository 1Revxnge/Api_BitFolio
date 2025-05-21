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
        public async Task<IActionResult> GetFuncById(int id)
        {
            var funcionario = await _funcionarioService.GetFuncionarioByIdAsync(id);
            if (funcionario == null)
                return NotFound();
            return Ok(funcionario);
        }
        
    }
}