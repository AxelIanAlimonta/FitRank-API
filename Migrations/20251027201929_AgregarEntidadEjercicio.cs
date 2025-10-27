using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEntidadEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Asistencias_Profesores_ProfesorId",
            //    table: "Asistencias");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Asistencias_Socios_SocioId",
            //    table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_GruposMusculares_GrupoMuscularId",
                table: "Ejercicios");

            //migrationBuilder.DropIndex(
            //    name: "IX_Asistencias_ProfesorId",
            //    table: "Asistencias");

            //migrationBuilder.DropIndex(
            //    name: "IX_Asistencias_SocioId",
            //    table: "Asistencias");

            //migrationBuilder.DropColumn(
            //    name: "ProfesorId",
            //    table: "Asistencias");

            //migrationBuilder.DropColumn(
            //    name: "SocioId",
            //    table: "Asistencias");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "GruposMusculares",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Imagen",
                table: "GruposMusculares",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UrlVideo",
                table: "Ejercicios",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ejercicios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Ejercicios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuracionEstimada",
                table: "Ejercicios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MaquinaId",
                table: "Ejercicios",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlImagen",
                table: "Ejercicios",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "UsuarioId",
                table: "Asistencias",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Sesiones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroDeSesion = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    RutinaId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sesiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sesiones_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ejercicios_MaquinaId",
                table: "Ejercicios",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioId",
                table: "Asistencias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesiones_RutinaId",
                table: "Sesiones",
                column: "RutinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId",
                table: "Asistencias",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_GruposMusculares_GrupoMuscularId",
                table: "Ejercicios",
                column: "GrupoMuscularId",
                principalTable: "GruposMusculares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_Maquinas_MaquinaId",
                table: "Ejercicios",
                column: "MaquinaId",
                principalTable: "Maquinas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asistencias_Usuarios_UsuarioId",
                table: "Asistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_GruposMusculares_GrupoMuscularId",
                table: "Ejercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_Maquinas_MaquinaId",
                table: "Ejercicios");

            migrationBuilder.DropTable(
                name: "Sesiones");

            migrationBuilder.DropIndex(
                name: "IX_Ejercicios_MaquinaId",
                table: "Ejercicios");

            migrationBuilder.DropIndex(
                name: "IX_Asistencias_UsuarioId",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "GruposMusculares");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Ejercicios");

            migrationBuilder.DropColumn(
                name: "DuracionEstimada",
                table: "Ejercicios");

            migrationBuilder.DropColumn(
                name: "MaquinaId",
                table: "Ejercicios");

            migrationBuilder.DropColumn(
                name: "UrlImagen",
                table: "Ejercicios");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "GruposMusculares",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "UrlVideo",
                table: "Ejercicios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ejercicios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<long>(
                name: "UsuarioId",
                table: "Asistencias",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            //migrationBuilder.AddColumn<long>(
            //    name: "ProfesorId",
            //    table: "Asistencias",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.AddColumn<long>(
            //    name: "SocioId",
            //    table: "Asistencias",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Asistencias_ProfesorId",
            //    table: "Asistencias",
            //    column: "ProfesorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Asistencias_SocioId",
            //    table: "Asistencias",
            //    column: "SocioId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Asistencias_Profesores_ProfesorId",
            //    table: "Asistencias",
            //    column: "ProfesorId",
            //    principalTable: "Profesores",
            //    principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Asistencias_Socios_SocioId",
            //    table: "Asistencias",
            //    column: "SocioId",
            //    principalTable: "Socios",
            //    principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_GruposMusculares_GrupoMuscularId",
                table: "Ejercicios",
                column: "GrupoMuscularId",
                principalTable: "GruposMusculares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
