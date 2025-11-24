using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiJobfy.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AdicionarUltimoAcessoParaFuncionarioAdministrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoAcesso",
                table: "Recrutadores",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoAcesso",
                table: "Administradores",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UltimoAcesso",
                table: "Recrutadores");

            migrationBuilder.DropColumn(
                name: "UltimoAcesso",
                table: "Administradores");
        }
    }
}
