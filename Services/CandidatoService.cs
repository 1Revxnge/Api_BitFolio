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

        public async Task<Candidato?> GetCandidatoByIdAsync(int id)
        {
            return await _dbContext.Candidatos
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateCandidatoAsync(Candidato candidato)
        {
            var existingCandidato = await _dbContext.Candidatos
                .FirstOrDefaultAsync(c => c.Id == candidato.Id);

            DateTime agora = DateTime.Now;
            if (existingCandidato == null)
                return false;

            existingCandidato.Nome = candidato.Nome;
            existingCandidato.Email = candidato.Email;
            existingCandidato.Telefone = candidato.Telefone;
            existingCandidato.CurriculoCriptografado = candidato.CurriculoCriptografado;
            candidato.DtAtualizacao = agora;
            // Atualize outros campos conforme necessário

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeleteCandidatoAsync(int id)
        {
            var candidato = await _dbContext.Candidatos
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidato == null)
                return false;

            candidato.Ativo = false; 
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}


