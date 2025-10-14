using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class CreacionBloque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BloqueId",
                table: "EjerciciosRealizados",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Bloque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RutinaId = table.Column<int>(type: "integer", nullable: false),
                    EjercicioId = table.Column<int>(type: "integer", nullable: false),
                    Dia = table.Column<int>(type: "integer", nullable: false),
                    SeriesRecomendadas = table.Column<int>(type: "integer", nullable: false),
                    RepeticionesRecomendadas = table.Column<int>(type: "integer", nullable: false),
                    PesoRecomendado = table.Column<double>(type: "double precision", nullable: false),
                    RirRecomendado = table.Column<int>(type: "integer", nullable: false),
                    Notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bloque_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bloque_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_BloqueId",
                table: "EjerciciosRealizados",
                column: "BloqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Bloque_EjercicioId",
                table: "Bloque",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Bloque_RutinaId",
                table: "Bloque",
                column: "RutinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Bloque_BloqueId",
                table: "EjerciciosRealizados",
                column: "BloqueId",
                principalTable: "Bloque",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Bloque_BloqueId",
                table: "EjerciciosRealizados");

            migrationBuilder.DropTable(
                name: "Bloque");

            migrationBuilder.DropIndex(
                name: "IX_EjerciciosRealizados_BloqueId",
                table: "EjerciciosRealizados");

            migrationBuilder.DropColumn(
                name: "BloqueId",
                table: "EjerciciosRealizados");
        }
    }
}
