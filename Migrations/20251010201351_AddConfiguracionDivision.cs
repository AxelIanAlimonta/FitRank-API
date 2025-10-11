using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dificultad",
                table: "EjerciciosRealizados",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
    ALTER TABLE ""Ejercicios""
    ALTER COLUMN ""GrupoMuscular"" TYPE integer USING
    CASE ""GrupoMuscular""
        WHEN 'Pecho' THEN 0
        WHEN 'Espalda' THEN 1
        WHEN 'Piernas' THEN 2
        WHEN 'Brazos' THEN 3
        WHEN 'Hombros' THEN 4
        WHEN 'Gluteos' THEN 5
        WHEN 'Abdominales' THEN 6
    END;
");

            migrationBuilder.AddColumn<int>(
                name: "Dificultad",
                table: "Ejercicios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConfiguracionesDivisiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    PuntosMinimos = table.Column<double>(type: "double precision", nullable: false),
                    PuntosMaximos = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesDivisiones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesDivisiones");

            migrationBuilder.DropColumn(
                name: "Dificultad",
                table: "EjerciciosRealizados");

            migrationBuilder.DropColumn(
                name: "Dificultad",
                table: "Ejercicios");

            migrationBuilder.AlterColumn<string>(
                name: "GrupoMuscular",
                table: "Ejercicios",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
