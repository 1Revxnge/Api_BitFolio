using ApiJobfy.models;

namespace ApiJobfy.Services.IService
{
    public interface IEnderecoService
    {
     Task<Endereco> AddEnderecoAsync(Endereco endereco, Guid? candidatoId = null, Guid? empresaId = null);
     Task<bool> UpdateEnderecoAsync(Endereco endereco);
     Task<bool> DeleteEnderecoAsync(Guid id);
     Task<Endereco> GetEnderecoByIdAsync(Guid id);
     }
}

