using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeJsonColumnsToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RulesExplainJson",
                table: "Rutinas",
                type: "text",
                nullable: true,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InputSnapshotJson",
                table: "Rutinas",
                type: "text",
                nullable: true,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<JsonDocument>(
                name: "RulesExplainJson",
                table: "Rutinas",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<JsonDocument>(
                name: "InputSnapshotJson",
                table: "Rutinas",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
