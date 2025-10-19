using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionGrupoMuscular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesGrupoMuscular",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GrupoMuscularId = table.Column<int>(type: "integer", nullable: false),
                    GrupoMuscularId1 = table.Column<long>(type: "bigint", nullable: false),
                    Multiplicadopeso = table.Column<double>(type: "double precision", nullable: false),
                    MultiplicadorRepeticiones = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesGrupoMuscular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesGrupoMuscular_GruposMusculares_GrupoMuscular~",
                        column: x => x.GrupoMuscularId1,
                        principalTable: "GruposMusculares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesGrupoMuscular_GrupoMuscularId1",
                table: "ConfiguracionesGrupoMuscular",
                column: "GrupoMuscularId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesGrupoMuscular");
        }
    }
}
