using ApiJobfy.Data;
using ApiJobfy.models;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Services.ApiJobfy.Services
{
    public class FuncionarioService : IFuncionarioService
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public FuncionarioService(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Obter todos os Funcionarios com paginação
        public async Task<IEnumerable<Funcionario>> GetFuncionariosAsync(int page, int pageSize)
        {
            return await _dbContext.Funcionarios
                .Skip((page - 1) * pageSize)  // Paginação
                .Take(pageSize)              // Limita os resultados
                .ToListAsync();
        }

        // Obter um Funcionario pelo ID
        public async Task<Funcionario?> GetFuncionarioByIdAsync(int id)
        {
            return await _dbContext.Funcionarios
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // Atualizar um Funcionario
        public async Task<bool> UpdateFuncionarioAsync(Funcionario funcionario)
        {
            var existingFuncionario = await _dbContext.Funcionarios
                .FirstOrDefaultAsync(f => f.Id == funcionario.Id);

            if (existingFuncionario == null)
                return false; // Retorna falso se não encontrar o Funcionario

            // Atualiza os dados do Funcionario
            existingFuncionario.Nome = funcionario.Nome;
            existingFuncionario.Email = funcionario.Email;
            existingFuncionario.Telefone = funcionario.Telefone;
            existingFuncionario.Cargo = funcionario.Cargo;

            // Salva as alterações no banco de dados
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Adicionar um novo Funcionario
        public async Task<Funcionario> AddFuncionarioAsync(Funcionario funcionario)
        {
            // Adiciona o novo Funcionario ao DbContext
            _dbContext.Funcionarios.Add(funcionario);
            await _dbContext.SaveChangesAsync();

            // Retorna o Funcionario adicionado com o ID gerado
            return funcionario;
        }

        // Excluir um Funcionario (soft delete)
        public async Task<bool> SoftDeleteFuncionarioAsync(int id)
        {
            var funcionario = await _dbContext.Funcionarios
                .FirstOrDefaultAsync(f => f.Id == id);

            if (funcionario == null)
                return false; // Retorna falso se não encontrar o Funcionario

            // Marca o Funcionario como "não ativo" (soft delete)
            funcionario.Ativo = false;  // Supondo que "Ativo" é um campo que controla a exclusão lógica

            // Salva as alterações no banco de dados
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}