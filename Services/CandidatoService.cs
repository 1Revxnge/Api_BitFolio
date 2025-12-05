using ApiJobfy.Data;
using ApiJobfy.models.DTOs;
using ApiJobfy.models;
using ApiJobfy.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ApiJobfy.Services
{
    [ExcludeFromCodeCoverage]

    public class CandidatoService : ICandidatoService
    {
        private readonly AppDbContext _dbContext;

        public CandidatoService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Candidato>> GetCandidatosAsync(int page, int take)
        {
            int skip = take * (page - 1);

            return await _dbContext.Candidatos
                .Where(c => c.Ativo == true) 
                .OrderBy(c => c.Nome)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalCandidatosAsync()
        {
            return await _dbContext.Candidatos
                .Where(c => c.Ativo) 
                .CountAsync();
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
            existingCandidato.DataNascimento = candidato.DataNascimento;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Cria ou atualiza o currículo de um candidato (1:1)
        /// </summary>
        public Curriculo CriarOuAtualizar(Guid candidatoId, Curriculo curriculo)
        {
            var candidato = _dbContext.Candidatos
                .FirstOrDefault(c => c.CandidatoId == candidatoId);

            if (candidato == null)
                throw new Exception("Candidato não encontrado.");

            if (candidato.CurriculoId.HasValue)
            {
                var existente = _dbContext.Curriculos
                    .First(c => c.CurriculoId == candidato.CurriculoId.Value);

                existente.Experiencias = curriculo.Experiencias;
                existente.Tecnologias = curriculo.Tecnologias;
                existente.CompetenciasTecnicas = curriculo.CompetenciasTecnicas;
                existente.Idiomas = curriculo.Idiomas;
                existente.Certificacoes = curriculo.Certificacoes;

                _dbContext.SaveChanges();
                return existente;
            }

            var novo = new Curriculo
            {
                CurriculoId = Guid.NewGuid(),
                Experiencias = curriculo.Experiencias,
                Tecnologias = curriculo.Tecnologias,
                CompetenciasTecnicas = curriculo.CompetenciasTecnicas,
                Idiomas = curriculo.Idiomas,
                Certificacoes = curriculo.Certificacoes
            };

            _dbContext.Curriculos.Add(novo);

            candidato.CurriculoId = novo.CurriculoId;

            _dbContext.SaveChanges();
            return novo;
        }

        /// <summary>
        /// Busca o currículo de um candidato
        /// </summary>
        public Curriculo? BuscarPorCandidato(Guid candidatoId)
        {
            // 1) busca o candidato
            var candidato = _dbContext.Candidatos
                .AsNoTracking()
                .FirstOrDefault(c => c.CandidatoId == candidatoId);

            if (candidato == null) return null;

            // 2) se o candidato tiver CurriculoId, retorna o currículo
            if (candidato.CurriculoId.HasValue)
            {
                return _dbContext.Curriculos
                    .AsNoTracking()
                    .FirstOrDefault(cur => cur.CurriculoId == candidato.CurriculoId.Value);
            }

            // sem currículo vinculado
            return null;
        }


        /// <summary>
        /// Deleta o currículo de um candidato
        /// </summary>
        public bool Deletar(Guid candidatoId)
        {
            var candidato = _dbContext.Candidatos
                .AsNoTracking()
                .FirstOrDefault(c => c.CandidatoId == candidatoId);

            if (candidato == null || !candidato.CurriculoId.HasValue)
                return false;

            var curriculo = _dbContext.Curriculos
                .FirstOrDefault(c => c.CurriculoId == candidato.CurriculoId.Value);

            if (curriculo == null)
                return false;

            _dbContext.Curriculos.Remove(curriculo);

            var candidatoUpdate = _dbContext.Candidatos
                .First(c => c.CandidatoId == candidatoId); 
            candidatoUpdate.CurriculoId = null;

            _dbContext.SaveChanges();
            return true;
        }

        public async Task<int> GetTotalLogsCandidatoAsync(Guid candidatoId)
        {
            return await _dbContext.LogCandidatos
        .Where(l =>
            l.CandidatoId == candidatoId &&
            l.Acao != "Login bem-sucedido" &&
            l.Acao != "Login falhou"
        )
        .CountAsync();
        }

        public async Task<List<LogCandidato>> GetLogsCandidatoAsync(Guid candidatoId, int page, int pageSize)
        {
            return await _dbContext.LogCandidatos
        .Where(l =>
            l.CandidatoId == candidatoId &&
            l.Acao != "Login bem-sucedido" &&
            l.Acao != "Login falhou"
        )
        .OrderByDescending(l => l.DtAcao)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(l => new LogCandidato
        {
            LogId = l.LogId,
            CandidatoId = l.CandidatoId,
            Acao = l.Acao,
            DtAcao = l.DtAcao
        })
        .ToListAsync();
        }


    }
}



