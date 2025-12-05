using ApiJobfy.models;
using BitFolio.models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace ApiJobfy.Data

    {   
        public class AppDbContext : DbContext
        {
            public DbSet<Candidato> Candidatos => Set<Candidato>();
            public DbSet<Recrutador> Recrutadores => Set<Recrutador>();
            public DbSet<Administrador> Administradores => Set<Administrador>();
            public DbSet<LogCandidato> LogCandidatos => Set<LogCandidato>();
            public DbSet<LogRecrutador> LogRecrutadores => Set<LogRecrutador>();
            public DbSet<LogAdministrador> LogAdministradores => Set<LogAdministrador>();
            public DbSet<Vaga> Vagas => Set<Vaga>();
            public DbSet<Empresa> Empresas => Set<Empresa>();
            public DbSet<Endereco> Enderecos => Set<Endereco>();
            public DbSet<Curriculo> Curriculos => Set<Curriculo>();
            public DbSet<CandidatoVaga> CandidatoVagas => Set<CandidatoVaga>();
            public DbSet<VagaFavorita> VagasFavoritas => Set<VagaFavorita>();
            public DbSet<TokenTemporario> TokenTemporario => Set<TokenTemporario>();
            public DbSet<HistoricoCandidatura> HistoricoCandidaturas => Set<HistoricoCandidatura>();
            public DbSet<SolicitacaoEmpresa> SolicitacoesEmpresa => Set<SolicitacaoEmpresa>();
            public DbSet<SolicitacaoEndereco> SolicitacoesEndereco => Set<SolicitacaoEndereco>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ========= Chaves Primárias =========
            modelBuilder.Entity<Candidato>().HasKey(c => c.CandidatoId);
            modelBuilder.Entity<Recrutador>().HasKey(r => r.RecrutadorId);
            modelBuilder.Entity<Administrador>().HasKey(a => a.AdminId);
            modelBuilder.Entity<Empresa>().HasKey(e => e.EmpresaId);
            modelBuilder.Entity<Endereco>().HasKey(e => e.EnderecoId);
            modelBuilder.Entity<Curriculo>().HasKey(c => c.CurriculoId);
            modelBuilder.Entity<Vaga>().HasKey(v => v.VagaId);
            modelBuilder.Entity<CandidatoVaga>().HasKey(cv => cv.Id);
            modelBuilder.Entity<VagaFavorita>().HasKey(vf => vf.Id);
            modelBuilder.Entity<TokenTemporario>().HasKey(t => t.Id);
            modelBuilder.Entity<LogCandidato>().HasKey(l => l.LogId);
            modelBuilder.Entity<LogRecrutador>().HasKey(l => l.LogId);
            modelBuilder.Entity<LogAdministrador>().HasKey(l => l.LogId);
            modelBuilder.Entity<HistoricoCandidatura>().HasKey(h => h.HistoricoId);
            modelBuilder.Entity<SolicitacaoEmpresa>().HasKey(s => s.SolicitacaoId);
            modelBuilder.Entity<SolicitacaoEndereco>().HasKey(s => s.SolicitacaoId);

            // ========= Índices Únicos =========
            modelBuilder.Entity<Candidato>().HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<Recrutador>().HasIndex(r => r.Email).IsUnique();
            modelBuilder.Entity<Administrador>().HasIndex(a => a.Email).IsUnique();
            modelBuilder.Entity<Empresa>().HasIndex(e => e.CNPJ).IsUnique();

            // ========= Logs =========
            modelBuilder.Entity<LogCandidato>()
                .HasOne(l => l.Candidato)
                .WithMany(c => c.Logs)
                .HasForeignKey(l => l.CandidatoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogRecrutador>()
                .HasOne(l => l.Recrutador)
                .WithMany(r => r.Logs)
                .HasForeignKey(l => l.RecrutadorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogAdministrador>()
                .HasOne(l => l.Administrador)
                .WithMany(a => a.Logs)
                .HasForeignKey(l => l.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========= Relacionamentos =========

            // Candidato → Curriculo (1:1)
            modelBuilder.Entity<Candidato>()
                .HasOne(c => c.Curriculo)
                .WithOne()
                .HasForeignKey<Candidato>(c => c.CurriculoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Empresa → Endereco (1:1)
            modelBuilder.Entity<Empresa>()
                .HasOne(e => e.Endereco)
                .WithOne(ed => ed.Empresa)
                .HasForeignKey<Empresa>(e => e.EnderecoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Endereco → Candidato (1:1)
            modelBuilder.Entity<Candidato>()
                .HasOne(c => c.Endereco)
                .WithOne(ed => ed.Candidato)
                .HasForeignKey<Candidato>(c => c.EnderecoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Vaga → Empresa (N:1)
            modelBuilder.Entity<Vaga>()
                .HasOne(v => v.Empresa)
                .WithMany(e => e.Vagas)
                .HasForeignKey(v => v.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            // CandidatoVaga (N:N simplificado)
            modelBuilder.Entity<CandidatoVaga>()
                .HasOne(cv => cv.Candidato)
                .WithMany(c => c.CandidatoVagas)
                .HasForeignKey(cv => cv.CandidatoId);

            modelBuilder.Entity<CandidatoVaga>()
                .HasOne(cv => cv.Vaga)
                .WithMany(v => v.CandidatoVagas)
                .HasForeignKey(cv => cv.VagaId);

            // VagaFavorita (N:N simplificado)
            modelBuilder.Entity<VagaFavorita>()
            .HasOne(vf => vf.Candidato)
            .WithMany(c => c.VagasFavoritas)
            .HasForeignKey(vf => vf.CandidatoId)
            .OnDelete(DeleteBehavior.Cascade);

            // VagaFavorita (N:1 com Vaga)
            modelBuilder.Entity<VagaFavorita>()
                .HasOne(vf => vf.Vaga)
                .WithMany(v => v.VagasFavoritas)
                .HasForeignKey(vf => vf.VagaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoricoCandidatura>()
            .HasOne(h => h.Candidato)
            .WithMany(c => c.Historicos)
            .HasForeignKey(h => h.CandidatoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoricoCandidatura>()
                .HasOne(h => h.Vaga)
                .WithMany(v => v.Historicos)
                .HasForeignKey(h => h.VagaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SolicitacaoEmpresa>()
           .HasOne(s => s.Empresa)
           .WithMany()
           .HasForeignKey(s => s.EmpresaId)
           .OnDelete(DeleteBehavior.Cascade);

            // SolicitacaoEndereco → Endereco (1:N)
            modelBuilder.Entity<SolicitacaoEndereco>()
                .HasOne<Endereco>()
                .WithMany()
                .HasForeignKey(s => s.EnderecoId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

    }
}


