using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace ApiJobfy.Controllers
{
    [ApiController]
    [Route("api/endereco")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpGet("getEnderecoById/{id}")]
        public async Task<IActionResult> GetEnderecoById(int id)
        {
            var endereco = await _enderecoService.GetEnderecoByIdAsync(id);
            if (endereco == null)
                return NotFound();

            return Ok(endereco);
        }

        [HttpPost("createEndereco")]
        public async Task<IActionResult> CreateEndereco([FromBody] Endereco endereco)
        {
            if (endereco == null)
                return BadRequest("Endereço não pode ser nulo.");

            var novoEndereco = await _enderecoService.AddEnderecoAsync(endereco);
            return CreatedAtAction(nameof(GetEnderecoById), new { id = novoEndereco.Id }, novoEndereco);
        }

        [HttpPut("updateEndereco")]
        public async Task<IActionResult> UpdateEndereco([FromBody] Endereco endereco)
        {
            if (endereco == null)
                return BadRequest("Endereço não pode ser nulo.");

            var resultado = await _enderecoService.UpdateEnderecoAsync(endereco);
            if (!resultado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("deleteEndereco/{id}")]
        public async Task<IActionResult> DeleteEndereco(int id)
        {
            var resultado = await _enderecoService.DeleteEnderecoAsync(id);
            if (!resultado)
                return NotFound();

            return NoContent();
        }
    }
}