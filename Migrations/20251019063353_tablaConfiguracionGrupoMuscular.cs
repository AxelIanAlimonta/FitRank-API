using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class tablaConfiguracionGrupoMuscular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionesGrupoMuscular_GruposMusculares_GrupoMuscular~",
                table: "ConfiguracionesGrupoMuscular");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesGrupoMuscular_GrupoMuscularId1",
                table: "ConfiguracionesGrupoMuscular");

            migrationBuilder.DropColumn(
                name: "GrupoMuscularId1",
                table: "ConfiguracionesGrupoMuscular");

            migrationBuilder.AlterColumn<long>(
                name: "GrupoMuscularId",
                table: "ConfiguracionesGrupoMuscular",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesGrupoMuscular_GrupoMuscularId",
                table: "ConfiguracionesGrupoMuscular",
                column: "GrupoMuscularId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionesGrupoMuscular_GruposMusculares_GrupoMuscular~",
                table: "ConfiguracionesGrupoMuscular",
                column: "GrupoMuscularId",
                principalTable: "GruposMusculares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracionesGrupoMuscular_GruposMusculares_GrupoMuscular~",
                table: "ConfiguracionesGrupoMuscular");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracionesGrupoMuscular_GrupoMuscularId",
                table: "ConfiguracionesGrupoMuscular");

            migrationBuilder.AlterColumn<int>(
                name: "GrupoMuscularId",
                table: "ConfiguracionesGrupoMuscular",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "GrupoMuscularId1",
                table: "ConfiguracionesGrupoMuscular",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesGrupoMuscular_GrupoMuscularId1",
                table: "ConfiguracionesGrupoMuscular",
                column: "GrupoMuscularId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracionesGrupoMuscular_GruposMusculares_GrupoMuscular~",
                table: "ConfiguracionesGrupoMuscular",
                column: "GrupoMuscularId1",
                principalTable: "GruposMusculares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
