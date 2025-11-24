using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiJobfy.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class FixVagaFavoritaRelacionamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_VagasFavoritas_Vagas_VagaId1",
            //    table: "VagasFavoritas");

            //migrationBuilder.DropIndex(
            //    name: "IX_VagasFavoritas_VagaId1",
            //    table: "VagasFavoritas");

            //migrationBuilder.DropColumn(
            //    name: "VagaId1",
            //    table: "VagasFavoritas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VagaId1",
                table: "VagasFavoritas",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VagasFavoritas_VagaId1",
                table: "VagasFavoritas",
                column: "VagaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_VagasFavoritas_Vagas_VagaId1",
                table: "VagasFavoritas",
                column: "VagaId1",
                principalTable: "Vagas",
                principalColumn: "VagaId");
        }
    }
}
