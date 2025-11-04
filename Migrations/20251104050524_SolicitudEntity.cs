using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class SolicitudEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesRutinaProfesor",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    ProfesorId = table.Column<long>(type: "bigint", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MensajeSocio = table.Column<string>(type: "text", nullable: true),
                    MensajeProfesor = table.Column<string>(type: "text", nullable: true),
                    RutinaId = table.Column<long>(type: "bigint", nullable: true),
                    Edad = table.Column<int>(type: "integer", nullable: false),
                    PesoKg = table.Column<double>(type: "double precision", nullable: false),
                    AlturaCm = table.Column<double>(type: "double precision", nullable: false),
                    Nivel = table.Column<string>(type: "text", nullable: false),
                    SesionesPorSemana = table.Column<int>(type: "integer", nullable: false),
                    MinutosPorSesion = table.Column<int>(type: "integer", nullable: false),
                    Objetivo = table.Column<string>(type: "text", nullable: false),
                    CalidadAlimentacion = table.Column<int>(type: "integer", nullable: false),
                    HorasSuenio = table.Column<int>(type: "integer", nullable: false),
                    DolorLumbar = table.Column<bool>(type: "boolean", nullable: false),
                    DolorRodilla = table.Column<bool>(type: "boolean", nullable: false),
                    DolorHombro = table.Column<bool>(type: "boolean", nullable: false),
                    CirugiaReciente = table.Column<bool>(type: "boolean", nullable: false),
                    Sincope = table.Column<bool>(type: "boolean", nullable: false),
                    Embarazo = table.Column<bool>(type: "boolean", nullable: false),
                    Hipertension = table.Column<bool>(type: "boolean", nullable: false),
                    HipertensionControlada = table.Column<bool>(type: "boolean", nullable: false),
                    Diabetes = table.Column<bool>(type: "boolean", nullable: false),
                    DolorToracico = table.Column<bool>(type: "boolean", nullable: false),
                    FrecuenciaCardiacaReposo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesRutinaProfesor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesRutinaProfesor_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesRutinaProfesor_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesRutinaProfesor_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRutinaProfesor_ProfesorId",
                table: "SolicitudesRutinaProfesor",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRutinaProfesor_RutinaId",
                table: "SolicitudesRutinaProfesor",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesRutinaProfesor_SocioId",
                table: "SolicitudesRutinaProfesor",
                column: "SocioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesRutinaProfesor");
        }
    }
}
