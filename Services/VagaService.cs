using System.Collections.Generic;
using System.Threading.Tasks;
using ApiJobfy.Data;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.EntityFrameworkCore;       

namespace ApiJobfy.Services
{
    public class VagaService : IVagaService
    {
        private readonly AppDbContext _dbContext;

        public VagaService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Vaga>> GetVagasAsync(int page, int pageSize)
        {
            return await _dbContext.Vagas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vaga>> GetVagasByEmpresaIdAsync(Guid Empresas)
        {
            return await _dbContext.Vagas
                .Where(v => v.EmpresaId == Empresas)
                .ToListAsync();
        }

        public async Task<Vaga?> GetVagaByIdAsync(Guid id)
        {
            return await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);
        }

        public async Task<Vaga> AddVagaAsync(Vaga vaga)
        {
            _dbContext.Vagas.Add(vaga);
            await _dbContext.SaveChangesAsync();
            return vaga;
        }

        public async Task<Vaga?> UpdateVagaAsync(Vaga vaga)
        {
            var existingVaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == vaga.VagaId);

            if (existingVaga == null)
                return null;

            existingVaga.Titulo = vaga.Titulo;
            existingVaga.Descricao = vaga.Descricao;
            existingVaga.Nivel = vaga.Nivel;
            existingVaga.Modelo = vaga.Modelo;
            existingVaga.Requisitos = vaga.Requisitos;
            existingVaga.DataAbertura = vaga.DataAbertura;
            existingVaga.DataFechamento = vaga.DataFechamento;


            await _dbContext.SaveChangesAsync();
            return existingVaga; 
        }

        public async Task<bool> DeleteVagaAsync(Guid id)
        {
            var vaga = await _dbContext.Vagas
                .FirstOrDefaultAsync(v => v.VagaId == id);

            if (vaga == null)
                return false;

            _dbContext.Vagas.Remove(vaga);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        //Favotirar Vagas + Listagem de Vagas Favoritadas p/ Candidato
        public async Task<bool> ToggleFavoritoAsync(Guid candidatoId, Guid vagaId)
        {
            var favorito = await _dbContext.VagasFavoritas
                .FirstOrDefaultAsync(f => f.CandidatoId == candidatoId && f.VagaId == vagaId);

            if (favorito != null)
            {
                // Se já existe, desfavorita
                _dbContext.VagasFavoritas.Remove(favorito);
            }
            else
            {
                // Se não existe, favorita
                var novoFavorito = new VagaFavorita
                {
                    Id = Guid.NewGuid(),
                    CandidatoId = candidatoId,
                    VagaId = vagaId
                };
                await _dbContext.VagasFavoritas.AddAsync(novoFavorito);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Vaga>> GetFavoritosByCandidatoAsync(Guid candidatoId)
        {
            return await _dbContext.VagasFavoritas
                .Where(f => f.CandidatoId == candidatoId)
                .Select(f => f.Vaga!)
                .ToListAsync();
        }

        public IEnumerable<Vaga> BuscarPorPalavrasChave(string palavrasChave)
        {
            if (string.IsNullOrWhiteSpace(palavrasChave))
                return _dbContext.Vagas.ToList();
            var termos = palavrasChave.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var query = _dbContext.Vagas.AsQueryable();
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
