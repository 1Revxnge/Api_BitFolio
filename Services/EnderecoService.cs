using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Services
{
    public class EnderecoService : IEnderecoService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public EnderecoService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Endereco> AddEnderecoAsync(Endereco endereco)
        {

            _dbContext.Enderecos.Add(endereco);
            await _dbContext.SaveChangesAsync();

            return endereco;
        }

        public async Task<bool> UpdateEnderecoAsync(Endereco endereco)
        {
            var existingEndereco = await _dbContext.Enderecos
                .FirstOrDefaultAsync(e => e.EnderecoId == endereco.EnderecoId);

            if (existingEndereco == null)
                return false;

            existingEndereco.Rua = endereco.Rua;
            existingEndereco.Numero = endereco.Numero;
            existingEndereco.Complemento = endereco.Complemento;
            existingEndereco.Bairro = endereco.Bairro;
            existingEndereco.Cidade = endereco.Cidade;
            existingEndereco.Estado = endereco.Estado;
            existingEndereco.Cep = endereco.Cep;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEnderecoAsync(Guid id)
        {
            var endereco = await _dbContext.Enderecos
                .FirstOrDefaultAsync(e => e.EnderecoId == id);

            if (endereco == null)
                return false;

            _dbContext.Enderecos.Remove(endereco);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Endereco> GetEnderecoByIdAsync(Guid id)
        {
            var endereco = await _dbContext.Enderecos
                .FirstOrDefaultAsync(e => e.EnderecoId == id);

            if (endereco == null)
                throw new Exception("Endereço não encontrado.");

            return endereco;
        }
    }
}