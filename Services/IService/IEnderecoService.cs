using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IEnderecoService
    {
     Task<Endereco> AddEnderecoAsync(Endereco endereco);
     Task<bool> UpdateEnderecoAsync(Endereco endereco);
     Task<bool> DeleteEnderecoAsync(int id);
     Task<Endereco> GetEnderecoByIdAsync(int id);
     }
}

