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

    }
}


