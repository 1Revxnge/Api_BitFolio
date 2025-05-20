using ApiJobfy.models;
using Microsoft.EntityFrameworkCore;

namespace ApiJobfy.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Candidato> Candidatos => Set<Candidato>();
        public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
        public DbSet<Administrador> Administradores => Set<Administrador>();
        public DbSet<LogCandidato> LogCandidatos => Set<LogCandidato>();
        public DbSet<LogFuncionarios> LogFuncionarios => Set<LogFuncionarios>();
        public DbSet<LogAdministrador> LogAdministrador => Set<LogAdministrador>();
        public DbSet<Permissoes> Permissoes => Set<Permissoes>();
        public DbSet<Vagas> Vagas => Set<Vagas>();
        public DbSet<Empresas> Empresas => Set<Empresas>();
        public DbSet<Endereco> Endereco => Set<Endereco>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações de unicidade
            modelBuilder.Entity<Candidato>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Email)
                .IsUnique();

            modelBuilder.Entity<Administrador>()
                .HasIndex(a => a.Email)
                .IsUnique();

            // LogCandidato → Candidato
            modelBuilder.Entity<LogCandidato>()
                .HasOne(l => l.Candidato)
                .WithMany(c => c.LogCandidatos)
                .HasForeignKey(l => l.CandidatoId)
                .OnDelete(DeleteBehavior.Cascade);

            // LogFuncionario → Funcionario
            modelBuilder.Entity<LogFuncionarios>()
                .HasOne(l => l.Funcionario)
                .WithMany(f => f.LogFuncionarios)
                .HasForeignKey(l => l.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // LogAdministrador → Administrador
            modelBuilder.Entity<LogAdministrador>()
                .HasOne(l => l.Administrador)
                .WithMany(a => a.LogAdministradores)
                .HasForeignKey(l => l.AdministradorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Permissao → Candidato
            modelBuilder.Entity<Permissoes>()
                .HasOne(p => p.Candidato)
                .WithMany(c => c.Permissoes)
                .HasForeignKey(p => p.CandidatoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Empresa → Endereco
            modelBuilder.Entity<Empresas>()
                .HasOne(e => e.Endereco)
                .WithMany(ed => ed.Empresas)
                .HasForeignKey(e => e.EnderecoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Vaga → Empresa
            modelBuilder.Entity<Vagas>()
                .HasOne(v => v.Empresa)
                .WithMany(e => e.Vagas)
                .HasForeignKey(v => v.NegocioId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}

