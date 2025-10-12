using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTipoDeEntrenamientoFromEjercicioRealizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDeEntrenamiento",
                table: "EjerciciosRealizados");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoDeEntrenamiento",
                table: "EjerciciosRealizados",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
