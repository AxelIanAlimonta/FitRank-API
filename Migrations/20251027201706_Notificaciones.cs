using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Notificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Gimnasios_GimnasioId",
                table: "Asistencias");

          ///  migrationBuilder.DropForeignKey(
            ///    name: "FK_Asistencias_Usuarios_UsuarioId",
            ///    table: "Asistencias");

        ///    migrationBuilder.DropForeignKey(
         ///       name: "FK_Asistencias_Usuarios_UsuarioId1",
          ///      table: "Asistencias");

       ///     migrationBuilder.DropIndex(
          ///      name: "IX_Asistencias_UsuarioId1",
        ///        table: "Asistencias");

      ///      migrationBuilder.DropColumn(
        ///        name: "UsuarioId1",
        ///        table: "Asistencias");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Gimnasios_GimnasioId",
                table: "Asistencias",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Asistencias_Gimnasios_GimnasioId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId",
                table: "Asistencias");

            migrationBuilder.AddColumn<long>(
                name: "UsuarioId1",
                table: "Asistencias",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioId1",
                table: "Asistencias",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Gimnasios_GimnasioId",
                table: "Asistencias",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId",
                table: "Asistencias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId1",
                table: "Asistencias",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}


