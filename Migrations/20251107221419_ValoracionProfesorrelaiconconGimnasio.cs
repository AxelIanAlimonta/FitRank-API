using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class ValoracionProfesorrelaiconconGimnasio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GimnasioId",
                table: "Profesores",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Valoraciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmisorId = table.Column<long>(type: "bigint", nullable: false),
                    ReceptorId = table.Column<long>(type: "bigint", nullable: false),
                    RutinaId = table.Column<long>(type: "bigint", nullable: true),
                    Puntaje = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valoraciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Valoraciones_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Valoraciones_Usuarios_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Valoraciones_Usuarios_ReceptorId",
                        column: x => x.ReceptorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_GimnasioId",
                table: "Profesores",
                column: "GimnasioId");

            migrationBuilder.CreateIndex(
                name: "IX_Valoraciones_EmisorId",
                table: "Valoraciones",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Valoraciones_ReceptorId",
                table: "Valoraciones",
                column: "ReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Valoraciones_RutinaId",
                table: "Valoraciones",
                column: "RutinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profesores_Gimnasios_GimnasioId",
                table: "Profesores",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profesores_Gimnasios_GimnasioId",
                table: "Profesores");

            migrationBuilder.DropTable(
                name: "Valoraciones");

            migrationBuilder.DropIndex(
                name: "IX_Profesores_GimnasioId",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "GimnasioId",
                table: "Profesores");
        }
    }
}
