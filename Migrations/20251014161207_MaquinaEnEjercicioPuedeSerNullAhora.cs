using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class MaquinaEnEjercicioPuedeSerNullAhora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_Maquinas_MaquinaId",
                table: "Ejercicios");

            migrationBuilder.AlterColumn<int>(
                name: "MaquinaId",
                table: "Ejercicios",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_Maquinas_MaquinaId",
                table: "Ejercicios",
                column: "MaquinaId",
                principalTable: "Maquinas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_Maquinas_MaquinaId",
                table: "Ejercicios");

            migrationBuilder.AlterColumn<int>(
                name: "MaquinaId",
                table: "Ejercicios",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_Maquinas_MaquinaId",
                table: "Ejercicios",
                column: "MaquinaId",
                principalTable: "Maquinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
