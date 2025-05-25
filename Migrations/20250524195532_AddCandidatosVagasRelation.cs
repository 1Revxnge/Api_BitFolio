using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiJobfy.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidatosVagasRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DtNascimento",
                table: "Funcionarios",
                type: "date",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DtDemissao",
                table: "Funcionarios",
                type: "date",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DtAdmissao",
                table: "Funcionarios",
                type: "date",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DtNascimento",
                table: "Administradores",
                type: "date",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DtAprovacao",
                table: "Administradores",
                type: "date",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CandidatosVagas",
                columns: table => new
                {
                    CandidatosInscritosId = table.Column<int>(type: "int", nullable: false),
                    VagasInscritasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatosVagas", x => new { x.CandidatosInscritosId, x.VagasInscritasId });
                    table.ForeignKey(
                        name: "FK_CandidatosVagas_Candidatos_CandidatosInscritosId",
                        column: x => x.CandidatosInscritosId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidatosVagas_Vagas_VagasInscritasId",
                        column: x => x.VagasInscritasId,
                        principalTable: "Vagas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatosVagas_VagasInscritasId",
                table: "CandidatosVagas",
                column: "VagasInscritasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidatosVagas");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DtNascimento",
                table: "Funcionarios",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DtDemissao",
                table: "Funcionarios",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DtAdmissao",
                table: "Funcionarios",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DtNascimento",
                table: "Administradores",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "DtAprovacao",
                table: "Administradores",
                type: "time(6)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
