using ApiJobfy.models;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Candidato> Candidatos => Set<Candidato>();
        public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
        public DbSet<Administrador> Administradores => Set<Administrador>();
        public DbSet<LogUsuario> LogUsuarios => Set<LogUsuario>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Keys & Inheritances
            modelBuilder.Entity<Usuario>()
                .HasDiscriminator<string>("Role")
                .HasValue<Candidato>("Candidato")
                .HasValue<Funcionario>("Funcionario")
                .HasValue<Administrador>("Administrador");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
