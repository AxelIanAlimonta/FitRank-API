using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class ModificaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Usuarios",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "PesoKg",
                table: "Usuarios",
                newName: "pesoKg");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Usuarios",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Nivel",
                table: "Usuarios",
                newName: "nivel");

            migrationBuilder.RenameColumn(
                name: "FechaNacimiento",
                table: "Usuarios",
                newName: "fechaNacimiento");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Usuarios",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Usuarios",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Apellidos",
                table: "Usuarios",
                newName: "apellidos");

            migrationBuilder.RenameColumn(
                name: "AlturaCm",
                table: "Usuarios",
                newName: "alturaCm");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "id");

            migrationBuilder.AddColumn<int>(
                name: "dni",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EjercicioRealizadoDTOEntrada",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EjercicioId = table.Column<int>(type: "integer", nullable: false),
                    Series = table.Column<int>(type: "integer", nullable: false),
                    Repeticiones = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    TipoEntrenamiento = table.Column<string>(type: "text", nullable: false),
                    Observacion = table.Column<string>(type: "text", nullable: false),
                    fecha = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EjercicioRealizadoDTOEntrada", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EjercicioRealizadoDTOEntrada_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioRealizadoDTOEntrada_UsuarioId",
                table: "EjercicioRealizadoDTOEntrada",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EjercicioRealizadoDTOEntrada");

            migrationBuilder.DropColumn(
                name: "dni",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "Usuarios",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "pesoKg",
                table: "Usuarios",
                newName: "PesoKg");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Usuarios",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "nivel",
                table: "Usuarios",
                newName: "Nivel");

            migrationBuilder.RenameColumn(
                name: "fechaNacimiento",
                table: "Usuarios",
                newName: "FechaNacimiento");

            migrationBuilder.RenameColumn(
                name: "estado",
                table: "Usuarios",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Usuarios",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "apellidos",
                table: "Usuarios",
                newName: "Apellidos");

            migrationBuilder.RenameColumn(
                name: "alturaCm",
                table: "Usuarios",
                newName: "AlturaCm");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");
        }
    }
}
