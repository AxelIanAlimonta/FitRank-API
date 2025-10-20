using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class AsistenciaRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId1",
                table: "Asistencias");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_UsuarioId1",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Asistencias");

            migrationBuilder.AlterColumn<long>(
                name: "UsuarioId",
                table: "Asistencias",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioId",
                table: "Asistencias",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId",
                table: "Asistencias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId",
                table: "Asistencias");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_UsuarioId",
                table: "Asistencias");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Asistencias",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "UsuarioId1",
                table: "Asistencias",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioId1",
                table: "Asistencias",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId1",
                table: "Asistencias",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
