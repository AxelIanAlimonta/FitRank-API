using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Ranking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SocioId",
                table: "Puntajes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Puntajes_SocioId",
                table: "Puntajes",
                column: "SocioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Puntajes_Usuarios_SocioId",
                table: "Puntajes",
                column: "SocioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Puntajes_Usuarios_SocioId",
                table: "Puntajes");

            migrationBuilder.DropIndex(
                name: "IX_Puntajes_SocioId",
                table: "Puntajes");

            migrationBuilder.DropColumn(
                name: "SocioId",
                table: "Puntajes");
        }
    }
}
