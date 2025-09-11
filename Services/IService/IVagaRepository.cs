using ApiJobfy.models;

namespace BitFolio.Services.IService
{
    public interface IVagaRepository
    {
        IEnumerable<Vaga> BuscarPorPalavrasChave(string palavrasChave);
    }
}
