using ApiJobfy.models;
using ApiJobfy.Services;
using ApiJobfy.Services.IService;
using BitFolio.models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Controllers
{
    [ExcludeFromCodeCoverage]
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
        public async Task<IActionResult> GetEnderecoById(Guid id)
        {
            var endereco = await _enderecoService.GetEnderecoByIdAsync(id);
            if (endereco == null)
                return NotFound();

            return Ok(endereco);
        }

        [HttpPost("createEndereco")]
        public async Task<IActionResult> CreateEndereco([FromBody] Endereco endereco, Guid? candidatoId = null, Guid? empresaId = null)
        {
            if (endereco == null)
                return BadRequest("Endereço não pode ser nulo.");

            var novoEndereco = await _enderecoService.AddEnderecoAsync(endereco, candidatoId, empresaId);
            return CreatedAtAction(nameof(GetEnderecoById), new { id = novoEndereco.EnderecoId }, novoEndereco);
        }

        [HttpPut("updateEndereco")]
        public async Task<IActionResult> UpdateEndereco([FromBody] Endereco endereco, Guid? candidatoId = null, Guid? empresaId = null)
        {
            if (endereco == null)
                return BadRequest("Endereço não pode ser nulo.");
            var enderecoId = endereco.EnderecoId;
            if (enderecoId != null)
            {
                if (candidatoId != null) {
                    var resultado = await _enderecoService.UpdateEnderecoAsync(endereco);
            }

            } else
            {
                var resultado = await _enderecoService.AddEnderecoAsync(endereco, candidatoId, empresaId);

            }

            return NoContent();
        }



    }
}