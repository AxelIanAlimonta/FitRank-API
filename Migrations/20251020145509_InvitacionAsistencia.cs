using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class InvitacionAsistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosAsignados_Socios_SocioId",
                table: "EjerciciosAsignados");

            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Socios_SocioId",
                table: "EjerciciosRealizados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Socios",
                table: "Socios");

            migrationBuilder.RenameTable(
                name: "Socios",
                newName: "Usuarios");

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CuotaPagadaHasta",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Usuarios",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Dni",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EsActivado",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EstaActivo",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GimnasioId",
                table: "Usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QrToken",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Telefono",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpira",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenRecuperacion",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "alturaCm",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "nivel",
                table: "Usuarios",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "pesoKg",
                table: "Usuarios",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "telefono",
                table: "Usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId1 = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Presente = table.Column<bool>(type: "boolean", nullable: false),
                    HoraEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: false),
                    GymId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencias_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gimnasio",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gimnasio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GymId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    DatosPrellenados = table.Column<string>(type: "text", nullable: false),
                    MetodoPago = table.Column<string>(type: "text", nullable: false),
                    MpPaymentId = table.Column<string>(type: "text", nullable: true),
                    CreadaEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiraEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    CuotaPagadaHasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitaciones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GimnasioId",
                table: "Usuarios",
                column: "GimnasioId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioId1",
                table: "Asistencias",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosAsignados_Usuarios_SocioId",
                table: "EjerciciosAsignados",
                column: "SocioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Usuarios_SocioId",
                table: "EjerciciosRealizados",
                column: "SocioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Gimnasio_GimnasioId",
                table: "Usuarios",
                column: "GimnasioId",
                principalTable: "Gimnasio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosAsignados_Usuarios_SocioId",
                table: "EjerciciosAsignados");

            migrationBuilder.DropForeignKey(
                name: "FK_EjerciciosRealizados_Usuarios_SocioId",
                table: "EjerciciosRealizados");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Gimnasio_GimnasioId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Gimnasio");

            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_GimnasioId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CuotaPagadaHasta",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EsActivado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EstaActivo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "GimnasioId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "QrToken",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenExpira",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "TokenRecuperacion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "alturaCm",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "nivel",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "pesoKg",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "telefono",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Socios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Socios",
                table: "Socios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosAsignados_Socios_SocioId",
                table: "EjerciciosAsignados",
                column: "SocioId",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EjerciciosRealizados_Socios_SocioId",
                table: "EjerciciosRealizados",
                column: "SocioId",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
