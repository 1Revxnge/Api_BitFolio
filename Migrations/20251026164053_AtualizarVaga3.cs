using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiJobfy.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarVaga3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Salario",
                table: "Vagas",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salario",
                table: "Vagas");
        }
    }
}
