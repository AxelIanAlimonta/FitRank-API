using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class HacerAsistenciaNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Asistencias_AsistenciaId",
                table: "EjerciciosRealizados");

            migrationBuilder.AlterColumn<int>(
                name: "AsistenciaId",
                table: "EjerciciosRealizados",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Asistencias_AsistenciaId",
                table: "EjerciciosRealizados",
                column: "AsistenciaId",
                principalTable: "Asistencias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Asistencias_AsistenciaId",
                table: "EjerciciosRealizados");

            migrationBuilder.AlterColumn<int>(
                name: "AsistenciaId",
                table: "EjerciciosRealizados",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Asistencias_AsistenciaId",
                table: "EjerciciosRealizados",
                column: "AsistenciaId",
                principalTable: "Asistencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
