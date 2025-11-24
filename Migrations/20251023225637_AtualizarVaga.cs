using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace ApiJobfy.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class AtualizarVaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tecnologias",
                table: "Vagas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tecnologias",
                table: "Vagas");
        }
    }
}
