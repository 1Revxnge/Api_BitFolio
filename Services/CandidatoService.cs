using ApiJobfy.Data;
using ApiJobfy.models.DTOs;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Services
{

    public class CandidatoService : ICandidatoService
    {
        private readonly AppDbContext _dbContext;

        public CandidatoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Candidato>> GetCandidatosAsync(int page, int pageSize)
        {
            return await _dbContext.Candidatos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Candidato?> GetCandidatoByIdAsync(Guid id)
        {
            return await _dbContext.Candidatos
                .FirstOrDefaultAsync(c => c.CandidatoId == id);
        }

        public async Task<bool> UpdateCandidatoAsync(Candidato candidato)
        {
            var existingCandidato = await _dbContext.Candidatos
                .FirstOrDefaultAsync(c => c.CandidatoId == candidato.CandidatoId);

            DateTime agora = DateTime.Now;
            if (existingCandidato == null)
                return false;

            existingCandidato.Nome = candidato.Nome;
            existingCandidato.Email = candidato.Email;
            existingCandidato.Telefone = candidato.Telefone;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Cria ou atualiza o currículo de um candidato (1:1)
        /// </summary>
        public Curriculo? CriarOuAtualizar(Guid candidatoId, Curriculo curriculo)
        {
            var candidato = _dbContext.Candidatos
                .Include(c => c.Curriculo)
                .FirstOrDefault(c => c.CandidatoId == candidatoId);

            if (candidato == null)
                throw new Exception("Candidato não encontrado.");

            if (candidato.Curriculo != null)
            {
                // Atualiza o currículo existente
                candidato.Curriculo.Experiencias = curriculo.Experiencias;
                candidato.Curriculo.Tecnologias = curriculo.Tecnologias;
                candidato.Curriculo.CompetenciasTecnicas = curriculo.CompetenciasTecnicas;
                candidato.Curriculo.Idiomas = curriculo.Idiomas;
                candidato.Curriculo.Certificacoes = curriculo.Certificacoes;

                _dbContext.Curriculos.Update(candidato.Curriculo);
            }
            else
            {
                // Cria um novo currículo
                curriculo.CurriculoId = Guid.NewGuid();
                candidato.Curriculo = curriculo;
                _dbContext.Curriculos.Add(curriculo);
            }

            _dbContext.SaveChanges();
            return candidato.Curriculo;
        }

        /// <summary>
        /// Busca o currículo de um candidato
        /// </summary>
        public Curriculo? BuscarPorCandidato(Guid candidatoId)
        {
            return _dbContext.Curriculos
                .Include(c => c.Candidato)
                .FirstOrDefault(c => c.Candidato.CandidatoId == candidatoId);
        }

        /// <summary>
        /// Edita o currículo existente de um candidato
        /// </summary>
        public Curriculo? Editar(Guid candidatoId, Curriculo curriculo)
        {
            var existente = _dbContext.Curriculos
                .Include(c => c.Candidato)
                .FirstOrDefault(c => c.Candidato.CandidatoId == candidatoId);

            if (existente == null)
                return null;

            existente.Experiencias = curriculo.Experiencias;
            existente.Tecnologias = curriculo.Tecnologias;
            existente.CompetenciasTecnicas = curriculo.CompetenciasTecnicas;
            existente.Idiomas = curriculo.Idiomas;
            existente.Certificacoes = curriculo.Certificacoes;

            _dbContext.Curriculos.Update(existente);
            _dbContext.SaveChanges();

            return existente;
        }

        /// <summary>
        /// Deleta o currículo de um candidato
        /// </summary>
        public bool Deletar(Guid candidatoId)
        {
            var existente = _dbContext.Curriculos
                .Include(c => c.Candidato)
                .FirstOrDefault(c => c.Candidato.CandidatoId == candidatoId);

            if (existente == null)
                return false;

            _dbContext.Curriculos.Remove(existente);
            _dbContext.SaveChanges();

            return true;
        }
    }
}



