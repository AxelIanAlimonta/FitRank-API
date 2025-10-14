using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Ejerciciotieneidlongahra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bloque_Ejercicios_EjercicioId",
                table: "Bloque");

            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Ejercicios_EjercicioId",
                table: "EjerciciosRealizados");

            migrationBuilder.DropIndex(
                name: "IX_EjerciciosRealizados_EjercicioId",
                table: "EjerciciosRealizados");

            migrationBuilder.DropIndex(
                name: "IX_Bloque_EjercicioId",
                table: "Bloque");

            migrationBuilder.AddColumn<long>(
                name: "EjercicioId1",
                table: "EjerciciosRealizados",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Ejercicios",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "EjercicioId1",
                table: "Bloque",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_EjercicioId1",
                table: "EjerciciosRealizados",
                column: "EjercicioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bloque_EjercicioId1",
                table: "Bloque",
                column: "EjercicioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bloque_Ejercicios_EjercicioId1",
                table: "Bloque",
                column: "EjercicioId1",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Ejercicios_EjercicioId1",
                table: "EjerciciosRealizados",
                column: "EjercicioId1",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bloque_Ejercicios_EjercicioId1",
                table: "Bloque");

            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Ejercicios_EjercicioId1",
                table: "EjerciciosRealizados");

            migrationBuilder.DropIndex(
                name: "IX_EjerciciosRealizados_EjercicioId1",
                table: "EjerciciosRealizados");

            migrationBuilder.DropIndex(
                name: "IX_Bloque_EjercicioId1",
                table: "Bloque");

            migrationBuilder.DropColumn(
                name: "EjercicioId1",
                table: "EjerciciosRealizados");

            migrationBuilder.DropColumn(
                name: "EjercicioId1",
                table: "Bloque");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Ejercicios",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_EjercicioId",
                table: "EjerciciosRealizados",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Bloque_EjercicioId",
                table: "Bloque",
                column: "EjercicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bloque_Ejercicios_EjercicioId",
                table: "Bloque",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Ejercicios_EjercicioId",
                table: "EjerciciosRealizados",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
