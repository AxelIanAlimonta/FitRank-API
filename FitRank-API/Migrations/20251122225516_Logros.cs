using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Logros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LogroSocio_GimnasioId",
                table: "LogroSocio",
                column: "GimnasioId");

            migrationBuilder.CreateIndex(
                name: "IX_LogroSocio_SocioId",
                table: "LogroSocio",
                column: "SocioId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogrosGimnasio_Gimnasios_GimnasioId",
                table: "LogrosGimnasio",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogroSocio_Gimnasios_GimnasioId",
                table: "LogroSocio",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogroSocio_Socios_SocioId",
                table: "LogroSocio",
                column: "SocioId",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogrosGimnasio_Gimnasios_GimnasioId",
                table: "LogrosGimnasio");

            migrationBuilder.DropForeignKey(
                name: "FK_LogroSocio_Gimnasios_GimnasioId",
                table: "LogroSocio");

            migrationBuilder.DropForeignKey(
                name: "FK_LogroSocio_Socios_SocioId",
                table: "LogroSocio");

            migrationBuilder.DropIndex(
                name: "IX_LogroSocio_GimnasioId",
                table: "LogroSocio");

            migrationBuilder.DropIndex(
                name: "IX_LogroSocio_SocioId",
                table: "LogroSocio");
        }
    }
}