using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class EjercicioRealizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EjerciciosRealizados",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EjercicioId = table.Column<long>(type: "bigint", nullable: false),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    RutinaId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EjerciciosRealizados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EjerciciosRealizados_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjerciciosRealizados_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjerciciosRealizados_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesRealizadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Repeticiones = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    Rir = table.Column<int>(type: "integer", nullable: false),
                    NumeroDeSerie = table.Column<int>(type: "integer", nullable: false),
                    EjercicioRealizadoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesRealizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesRealizadas_EjerciciosRealizados_EjercicioRealizadoId",
                        column: x => x.EjercicioRealizadoId,
                        principalTable: "EjerciciosRealizados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Puntajes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerieRealizadaId = table.Column<long>(type: "bigint", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puntajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puntajes_SeriesRealizadas_SerieRealizadaId",
                        column: x => x.SerieRealizadaId,
                        principalTable: "SeriesRealizadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_EjercicioId",
                table: "EjerciciosRealizados",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_RutinaId",
                table: "EjerciciosRealizados",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_SocioId",
                table: "EjerciciosRealizados",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Puntajes_SerieRealizadaId",
                table: "Puntajes",
                column: "SerieRealizadaId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesRealizadas_EjercicioRealizadoId",
                table: "SeriesRealizadas",
                column: "EjercicioRealizadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Puntajes");

            migrationBuilder.DropTable(
                name: "SeriesRealizadas");

            migrationBuilder.DropTable(
                name: "EjerciciosRealizados");
        }
    }
}
