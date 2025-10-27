using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class Rutina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Socios_SocioId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Dificultades_DificultadId",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "Frecuencia",
                table: "Rutinas");

            migrationBuilder.RenameColumn(
                name: "DificultadId",
                table: "Rutinas",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Rutinas_DificultadId",
                table: "Rutinas",
                newName: "IX_Rutinas_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "SocioId",
                table: "Asistencias",
                newName: "UsuarioId1");

            migrationBuilder.RenameIndex(
                name: "IX_Asistencias_SocioId",
                table: "Asistencias",
                newName: "IX_Asistencias_UsuarioId1");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rutinas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activa",
                table: "Rutinas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Rutinas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Rutinas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "SocioId",
                table: "Rutinas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TipoCreacion",
                table: "Rutinas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "UsuarioId1",
                table: "Rutinas",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_SocioId",
                table: "Rutinas",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_UsuarioId1",
                table: "Rutinas",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId1",
                table: "Asistencias",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Socios_SocioId",
                table: "Rutinas",
                column: "SocioId",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Usuarios_UsuarioId",
                table: "Rutinas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Usuarios_UsuarioId1",
                table: "Rutinas",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId1",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Socios_SocioId",
                table: "Rutinas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Usuarios_UsuarioId",
                table: "Rutinas");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Usuarios_UsuarioId1",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_Rutinas_SocioId",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_Rutinas_UsuarioId1",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "Activa",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "SocioId",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "TipoCreacion",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Rutinas");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Rutinas",
                newName: "DificultadId");

            migrationBuilder.RenameIndex(
                name: "IX_Rutinas_UsuarioId",
                table: "Rutinas",
                newName: "IX_Rutinas_DificultadId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId1",
                table: "Asistencias",
                newName: "SocioId");

            migrationBuilder.RenameIndex(
                name: "IX_Asistencias_UsuarioId1",
                table: "Asistencias",
                newName: "IX_Asistencias_SocioId");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rutinas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "Frecuencia",
                table: "Rutinas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Socios_SocioId",
                table: "Asistencias",
                column: "SocioId",
                principalTable: "Socios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Dificultades_DificultadId",
                table: "Rutinas",
                column: "DificultadId",
                principalTable: "Dificultades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
