using ApiJobfy.models;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Data
{
    public class AppDbContext : DbContext
    {
     //   public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Candidato> Candidatos => Set<Candidato>();
        public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
        public DbSet<Administrador> Administradores => Set<Administrador>();
        public DbSet<LogUsuario> LogUsuarios => Set<LogUsuario>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações específicas para Candidato
            modelBuilder.Entity<Candidato>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Configurações específicas para Funcionario
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Email)
                .IsUnique();

            // Configurações específicas para Administrador
            modelBuilder.Entity<Administrador>()
                .HasIndex(a => a.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
