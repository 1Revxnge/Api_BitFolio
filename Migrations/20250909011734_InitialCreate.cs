using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiJobfy.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "Curriculos",
                columns: table => new
                {
                    CurriculoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Experiencias = table.Column<string>(type: "text", nullable: true),
                    Tecnologias = table.Column<string>(type: "text", nullable: true),
                    CompetenciasTecnicas = table.Column<string>(type: "text", nullable: true),
                    Idiomas = table.Column<string>(type: "text", nullable: true),
                    Certificacoes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curriculos", x => x.CurriculoId);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cep = table.Column<string>(type: "text", nullable: true),
                    Rua = table.Column<string>(type: "text", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.EnderecoId);
                });

            migrationBuilder.CreateTable(
                name: "TokenTemporario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiraEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Utilizado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenTemporario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogAdministradores",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminId = table.Column<Guid>(type: "uuid", nullable: false),
                    Acao = table.Column<string>(type: "text", nullable: false),
                    DtAcao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAdministradores", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_LogAdministradores_Administradores_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Administradores",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    CandidatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    UltimoAcesso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurriculoId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurriculoId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.CandidatoId);
                    table.ForeignKey(
                        name: "FK_Candidatos_Curriculos_CurriculoId",
                        column: x => x.CurriculoId,
                        principalTable: "Curriculos",
                        principalColumn: "CurriculoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Candidatos_Curriculos_CurriculoId1",
                        column: x => x.CurriculoId1,
                        principalTable: "Curriculos",
                        principalColumn: "CurriculoId");
                    table.ForeignKey(
                        name: "FK_Candidatos_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "EnderecoId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    RazaoSocial = table.Column<string>(type: "text", nullable: true),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.EmpresaId);
                    table.ForeignKey(
                        name: "FK_Empresas_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "EnderecoId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LogCandidatos",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Acao = table.Column<string>(type: "text", nullable: false),
                    DtAcao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogCandidatos", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_LogCandidatos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "CandidatoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recrutadores",
                columns: table => new
                {
                    RecrutadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Senha = table.Column<string>(type: "text", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recrutadores", x => x.RecrutadorId);
                    table.ForeignKey(
                        name: "FK_Recrutadores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "EmpresaId");
                });

            migrationBuilder.CreateTable(
                name: "Vagas",
                columns: table => new
                {
                    VagaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Nivel = table.Column<string>(type: "text", nullable: true),
                    Escolaridade = table.Column<string>(type: "text", nullable: true),
                    Modelo = table.Column<string>(type: "text", nullable: true),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataFechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Requisitos = table.Column<string>(type: "text", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vagas", x => x.VagaId);
                    table.ForeignKey(
                        name: "FK_Vagas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogRecrutadores",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecrutadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Acao = table.Column<string>(type: "text", nullable: false),
                    DtAcao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecrutadores", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_LogRecrutadores_Recrutadores_RecrutadorId",
                        column: x => x.RecrutadorId,
                        principalTable: "Recrutadores",
                        principalColumn: "RecrutadorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidatoVagas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    VagaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatoVagas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidatoVagas_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "CandidatoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidatoVagas_Vagas_VagaId",
                        column: x => x.VagaId,
                        principalTable: "Vagas",
                        principalColumn: "VagaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VagasFavoritas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidatoId = table.Column<Guid>(type: "uuid", nullable: false),
                    VagaId = table.Column<Guid>(type: "uuid", nullable: false),
                    VagaId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VagasFavoritas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VagasFavoritas_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "CandidatoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VagasFavoritas_Vagas_VagaId",
                        column: x => x.VagaId,
                        principalTable: "Vagas",
                        principalColumn: "VagaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VagasFavoritas_Vagas_VagaId1",
                        column: x => x.VagaId1,
                        principalTable: "Vagas",
                        principalColumn: "VagaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_Email",
                table: "Administradores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_CurriculoId",
                table: "Candidatos",
                column: "CurriculoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_CurriculoId1",
                table: "Candidatos",
                column: "CurriculoId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_Email",
                table: "Candidatos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_EnderecoId",
                table: "Candidatos",
                column: "EnderecoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoVagas_CandidatoId",
                table: "CandidatoVagas",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatoVagas_VagaId",
                table: "CandidatoVagas",
                column: "VagaId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_CNPJ",
                table: "Empresas",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_EnderecoId",
                table: "Empresas",
                column: "EnderecoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogAdministradores_AdminId",
                table: "LogAdministradores",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_LogCandidatos_CandidatoId",
                table: "LogCandidatos",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogRecrutadores_RecrutadorId",
                table: "LogRecrutadores",
                column: "RecrutadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recrutadores_Email",
                table: "Recrutadores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recrutadores_EmpresaId",
                table: "Recrutadores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vagas_EmpresaId",
                table: "Vagas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_VagasFavoritas_CandidatoId",
                table: "VagasFavoritas",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_VagasFavoritas_VagaId",
                table: "VagasFavoritas",
                column: "VagaId");

            migrationBuilder.CreateIndex(
                name: "IX_VagasFavoritas_VagaId1",
                table: "VagasFavoritas",
                column: "VagaId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatoVagas");

            migrationBuilder.DropTable(
                name: "LogAdministradores");

            migrationBuilder.DropTable(
                name: "LogCandidatos");

            migrationBuilder.DropTable(
                name: "LogRecrutadores");

            migrationBuilder.DropTable(
                name: "TokenTemporario");

            migrationBuilder.DropTable(
                name: "VagasFavoritas");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Recrutadores");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "Vagas");

            migrationBuilder.DropTable(
                name: "Curriculos");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Enderecos");
        }
    }
}
