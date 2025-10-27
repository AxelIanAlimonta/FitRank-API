using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogroEntityCreateLogroSocioEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Global",
                table: "Logros");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Logros",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreClave",
                table: "Logros",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Puntos",
                table: "Logros",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LogrosSocios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogroId = table.Column<long>(type: "bigint", nullable: false),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    PuntosOtorgados = table.Column<int>(type: "integer", nullable: false),
                    FechaOtorgado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogrosSocios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogrosSocios_Logros_LogroId",
                        column: x => x.LogroId,
                        principalTable: "Logros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogrosSocios_LogroId",
                table: "LogrosSocios",
                column: "LogroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogrosSocios");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Logros");

            migrationBuilder.DropColumn(
                name: "NombreClave",
                table: "Logros");

            migrationBuilder.DropColumn(
                name: "Puntos",
                table: "Logros");

            migrationBuilder.AddColumn<bool>(
                name: "Global",
                table: "Logros",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
