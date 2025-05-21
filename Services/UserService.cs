using ApiJobfy.Data;
using ApiJobfy.Services.IService;
using Microsoft.EntityFrameworkCore;


namespace ApiJobfy.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {

            // Verifica se o email existe na tabela de Candidatos
            bool candidatoExiste = await _dbContext.Candidatos
                                                   .AnyAsync(c => c.Email.ToLower() == email.ToLower());

            if (candidatoExiste)
                return true;

            // Verifica se o email existe na tabela de Administradores
            bool administradorExiste = await _dbContext.Administradores
                                                       .AnyAsync(a => a.Email.ToLower() == email.ToLower());

            if (administradorExiste)
                return true;

            // Verifica se o email existe na tabela de Funcionarios
            bool funcionarioExiste = await _dbContext.Funcionarios
                                                      .AnyAsync(f => f.Email.ToLower() == email.ToLower());

            return funcionarioExiste;

        }

    }
}
