using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionesParaRutinasIARutinaYEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<JsonDocument>(
                name: "InputSnapshotJson",
                table: "Rutinas",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "RulesExplainJson",
                table: "Rutinas",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "ContraIndicaciones",
                table: "Ejercicios",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "EquipoNecesario",
                table: "Ejercicios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<List<string>>(
                name: "Tags",
                table: "Ejercicios",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Ejercicios",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputSnapshotJson",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "RulesExplainJson",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "ContraIndicaciones",
                table: "Ejercicios");

            migrationBuilder.DropColumn(
                name: "EquipoNecesario",
                table: "Ejercicios");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Ejercicios");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Ejercicios");
        }
    }
}
