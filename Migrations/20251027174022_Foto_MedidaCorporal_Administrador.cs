using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Foto_MedidaCorporal_Administrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdministradorId",
                table: "Gimnasios",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AdministradorId",
                table: "Asistencias",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Cuil = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    Localidad = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administradores_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fotos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UrlImagen = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fotos_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedidasCorporales",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PechoCm = table.Column<double>(type: "double precision", nullable: false),
                    CinturaCm = table.Column<double>(type: "double precision", nullable: false),
                    CaderaCm = table.Column<double>(type: "double precision", nullable: false),
                    PesoKg = table.Column<double>(type: "double precision", nullable: false),
                    BrazoDerechoCm = table.Column<double>(type: "double precision", nullable: false),
                    BrazoIzquierdoCm = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedidasCorporales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedidasCorporales_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gimnasios_AdministradorId",
                table: "Gimnasios",
                column: "AdministradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_AdministradorId",
                table: "Asistencias",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotos_SocioId",
                table: "Fotos",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_MedidasCorporales_SocioId",
                table: "MedidasCorporales",
                column: "SocioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Administradores_AdministradorId",
                table: "Asistencias",
                column: "AdministradorId",
                principalTable: "Administradores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gimnasios_Administradores_AdministradorId",
                table: "Gimnasios",
                column: "AdministradorId",
                principalTable: "Administradores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Administradores_AdministradorId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Gimnasios_Administradores_AdministradorId",
                table: "Gimnasios");

            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Fotos");

            migrationBuilder.DropTable(
                name: "MedidasCorporales");

            migrationBuilder.DropIndex(
                name: "IX_Gimnasios_AdministradorId",
                table: "Gimnasios");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_AdministradorId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "AdministradorId",
                table: "Gimnasios");

            migrationBuilder.DropColumn(
                name: "AdministradorId",
                table: "Asistencias");
        }
    }
}
