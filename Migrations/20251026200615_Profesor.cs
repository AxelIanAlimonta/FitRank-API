using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Profesor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProfesorId",
                table: "Asistencias",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Profesores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Matricula = table.Column<string>(type: "text", nullable: false),
                    Sueldo = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_ProfesorId",
                table: "Asistencias",
                column: "ProfesorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Profesores_ProfesorId",
                table: "Asistencias",
                column: "ProfesorId",
                principalTable: "Profesores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Profesores_ProfesorId",
                table: "Asistencias");

            migrationBuilder.DropTable(
                name: "Profesores");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_ProfesorId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "ProfesorId",
                table: "Asistencias");
        }
    }
}
