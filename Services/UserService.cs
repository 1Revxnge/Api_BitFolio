using ApiJobfy.Data;
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
            return await _dbContext.Candidatos.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
