using ApiJobfy.Data;
using ApiJobfy.models;
using BitFolio.Services.IService;

namespace BitFolio.Services
{
    public class VagaRepository : IVagaRepository
    {
        private readonly AppDbContext _context;
        public VagaRepository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Vaga> BuscarPorPalavrasChave(string palavrasChave)
        {
            if (string.IsNullOrWhiteSpace(palavrasChave))
                return _context.Vagas.ToList();
            var termos = palavrasChave.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var query = _context.Vagas.AsQueryable();
            foreach (var termo in termos)
            {
                var temp = termo.ToLower();
                query = query.Where(v =>
                v.Titulo != null && v.Titulo.ToLower().Contains(temp) ||
                (v.Descricao ?? "").ToLower().Contains(temp));
            }
            return query.ToList();
        }
    }
}
