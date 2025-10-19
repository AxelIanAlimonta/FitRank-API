using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class tablaSeriesAsignadasAgregadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeriesAsignadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Peso = table.Column<int>(type: "integer", nullable: false),
                    Repeticiones = table.Column<int>(type: "integer", nullable: false),
                    Rir = table.Column<int>(type: "integer", nullable: false),
                    NroSerie = table.Column<int>(type: "integer", nullable: false),
                    EjercicioAsignadoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesAsignadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesAsignadas_EjerciciosAsignados_EjercicioAsignadoId",
                        column: x => x.EjercicioAsignadoId,
                        principalTable: "EjerciciosAsignados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeriesAsignadas_EjercicioAsignadoId",
                table: "SeriesAsignadas",
                column: "EjercicioAsignadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeriesAsignadas");
        }
    }
}
