using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class GimnasioEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Gimnasio_GimnasioId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitaciones_Gimnasio_GimnasioId",
                table: "Invitaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Socios_Gimnasio_GimnasioId",
                table: "Socios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gimnasio",
                table: "Gimnasio");

            migrationBuilder.RenameTable(
                name: "Gimnasio",
                newName: "Gimnasios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gimnasios",
                table: "Gimnasios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Gimnasios_GimnasioId",
                table: "Asistencias",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitaciones_Gimnasios_GimnasioId",
                table: "Invitaciones",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Socios_Gimnasios_GimnasioId",
                table: "Socios",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Gimnasios_GimnasioId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitaciones_Gimnasios_GimnasioId",
                table: "Invitaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Socios_Gimnasios_GimnasioId",
                table: "Socios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gimnasios",
                table: "Gimnasios");

            migrationBuilder.RenameTable(
                name: "Gimnasios",
                newName: "Gimnasio");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gimnasio",
                table: "Gimnasio",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Gimnasio_GimnasioId",
                table: "Asistencias",
                column: "GimnasioId",
                principalTable: "Gimnasio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitaciones_Gimnasio_GimnasioId",
                table: "Invitaciones",
                column: "GimnasioId",
                principalTable: "Gimnasio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Socios_Gimnasio_GimnasioId",
                table: "Socios",
                column: "GimnasioId",
                principalTable: "Gimnasio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
