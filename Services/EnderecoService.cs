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
            endereco.DtCriacao = DateTime.UtcNow;

            _dbContext.Endereco.Add(endereco);
            await _dbContext.SaveChangesAsync();

            return endereco;
        }

        public async Task<bool> UpdateEnderecoAsync(Endereco endereco)
        {
            var existingEndereco = await _dbContext.Endereco
                .FirstOrDefaultAsync(e => e.Id == endereco.Id);

            if (existingEndereco == null)
                return false;

            existingEndereco.Rua = endereco.Rua;
            existingEndereco.Numero = endereco.Numero;
            existingEndereco.Complemento = endereco.Complemento;
            existingEndereco.Bairro = endereco.Bairro;
            existingEndereco.Cidade = endereco.Cidade;
            existingEndereco.Estado = endereco.Estado;
            existingEndereco.Cep = endereco.Cep;
            existingEndereco.DtAtualizacao = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEnderecoAsync(int id)
        {
            var endereco = await _dbContext.Endereco
                .FirstOrDefaultAsync(e => e.Id == id);

            if (endereco == null)
                return false;

            _dbContext.Endereco.Remove(endereco);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Endereco?> GetEnderecoByIdAsync(int id)
        {
            return await _dbContext.Endereco
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}