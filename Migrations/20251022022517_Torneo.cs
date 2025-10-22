using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Torneo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Puntajes_SeriesRealizadas_SerieRealizadaId",
                table: "Puntajes");

            migrationBuilder.DropIndex(
                name: "IX_Puntajes_SerieRealizadaId",
                table: "Puntajes");

            migrationBuilder.DropColumn(
                name: "SerieRealizadaId",
                table: "Puntajes");

            migrationBuilder.CreateTable(
                name: "Torneos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    ParticipantesJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torneos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Torneos");

            migrationBuilder.AddColumn<long>(
                name: "SerieRealizadaId",
                table: "Puntajes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Puntajes_SerieRealizadaId",
                table: "Puntajes",
                column: "SerieRealizadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Puntajes_SeriesRealizadas_SerieRealizadaId",
                table: "Puntajes",
                column: "SerieRealizadaId",
                principalTable: "SeriesRealizadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
