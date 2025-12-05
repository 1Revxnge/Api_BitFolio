using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiJobfy.Migrations
{
    /// <inheritdoc />
    public partial class CriarSolicitacaoAlteracaoEmpresa2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitacoesEmpresa",
                columns: table => new
                {
                    SolicitacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmpresaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeNovo = table.Column<string>(type: "text", nullable: true),
                    RazaoSocialNova = table.Column<string>(type: "text", nullable: true),
                    DescricaoNova = table.Column<string>(type: "text", nullable: true),
                    DataSolicitacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Aprovado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesEmpresa", x => x.SolicitacaoId);
                    table.ForeignKey(
                        name: "FK_SolicitacoesEmpresa_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "EmpresaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitacoesEndereco",
                columns: table => new
                {
                    SolicitacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnderecoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RuaNova = table.Column<string>(type: "text", nullable: false),
                    NumeroNovo = table.Column<string>(type: "text", nullable: false),
                    ComplementoNovo = table.Column<string>(type: "text", nullable: false),
                    BairroNovo = table.Column<string>(type: "text", nullable: false),
                    CidadeNova = table.Column<string>(type: "text", nullable: false),
                    EstadoNovo = table.Column<string>(type: "text", nullable: false),
                    CepNovo = table.Column<string>(type: "text", nullable: false),
                    LatitudeNova = table.Column<double>(type: "double precision", nullable: true),
                    LongitudeNova = table.Column<double>(type: "double precision", nullable: true),
                    DataSolicitacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Aprovado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesEndereco", x => x.SolicitacaoId);
                    table.ForeignKey(
                        name: "FK_SolicitacoesEndereco_Enderecos_EnderecoId",
                        column: x => x.EnderecoId,
                        principalTable: "Enderecos",
                        principalColumn: "EnderecoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEmpresa_EmpresaId",
                table: "SolicitacoesEmpresa",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesEndereco_EnderecoId",
                table: "SolicitacoesEndereco",
                column: "EnderecoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitacoesEmpresa");

            migrationBuilder.DropTable(
                name: "SolicitacoesEndereco");
        }
    }
}
