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

        public async Task<Endereco> AddEnderecoAsync(Endereco endereco, Guid? candidatoId = null, Guid? empresaId = null)
        {
            _dbContext.Enderecos.Add(endereco);
            await _dbContext.SaveChangesAsync();

            if (candidatoId.HasValue)
            {
                var candidato = await _dbContext.Candidatos.FindAsync(candidatoId.Value);
                if (candidato != null)
                {
                    candidato.EnderecoId = endereco.EnderecoId;
                    await _dbContext.SaveChangesAsync();
                }
            }
            else if (empresaId.HasValue)
            {
                var empresa = await _dbContext.Empresas.FindAsync(empresaId.Value);
                if (empresa != null)
                {
                    empresa.EnderecoId = endereco.EnderecoId;
                    await _dbContext.SaveChangesAsync();
                }
            }

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
            existingEndereco.Latitude = endereco.Latitude;
            existingEndereco.Longitude = endereco.Longitude;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEnderecoAsync(Guid enderecoId)
        {
            // Remove vínculo do candidato
            var candidato = await _dbContext.Candidatos
                .FirstOrDefaultAsync(c => c.EnderecoId == enderecoId);
            if (candidato != null)
            {
                candidato.EnderecoId = null;
            }

            // Remove vínculo da empresa
            var empresa = await _dbContext.Empresas
                .FirstOrDefaultAsync(e => e.EnderecoId == enderecoId);
            if (empresa != null)
            {
                empresa.EnderecoId = null;
            }

            var endereco = await _dbContext.Enderecos.FindAsync(enderecoId);
            if (endereco != null)
            {
                _dbContext.Enderecos.Remove(endereco);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            await _dbContext.SaveChangesAsync();
            return false;
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