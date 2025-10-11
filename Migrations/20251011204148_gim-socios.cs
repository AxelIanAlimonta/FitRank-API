using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class gimsocios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Crear Gimnasios primero
            migrationBuilder.CreateTable(
                name: "Gimnasios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Gimnasios", x => x.Id); });

            // (opcional) crear un gimnasio real por defecto y guardarte su Id
            migrationBuilder.Sql(@"
        INSERT INTO ""Gimnasios"" (""Nombre"", ""Direccion"", ""Telefono"", ""Email"")
        VALUES ('Gimnasio General', '-', '-', '-');
    ");

            // 2) Agregar GimnasioId como NULLABLE y sin default
            migrationBuilder.AddColumn<int?>(
                name: "GimnasioId",
                table: "Socio",
                type: "integer",
                nullable: true);

            // 3) Backfill: asignar a todos los Socio el gimnasio recién creado
            migrationBuilder.Sql(@"
        UPDATE ""Socio""
        SET ""GimnasioId"" = (SELECT ""Id"" FROM ""Gimnasios"" ORDER BY ""Id"" LIMIT 1)
        WHERE ""GimnasioId"" IS NULL;
    ");

            // 4) Volver NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "GimnasioId",
                table: "Socio",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            // 5) Índices y FK
            migrationBuilder.CreateIndex(
                name: "IX_Socio_GimnasioId",
                table: "Socio",
                column: "GimnasioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Socio_Gimnasios_GimnasioId",
                table: "Socio",
                column: "GimnasioId",
                principalTable: "Gimnasios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Repetir el mismo patrón (nullable -> backfill -> not null -> FK) para SocioRealizaLogro.GimnasioId
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socio_Gimnasios_GimnasioId",
                table: "Socio");

            migrationBuilder.DropForeignKey(
                name: "FK_SocioRealizaLogro_Gimnasios_GimnasioId",
                table: "SocioRealizaLogro");

            migrationBuilder.DropTable(
                name: "GimnasioLogro");

            migrationBuilder.DropTable(
                name: "Gimnasios");

            migrationBuilder.DropIndex(
                name: "IX_SocioRealizaLogro_GimnasioId",
                table: "SocioRealizaLogro");

            migrationBuilder.DropIndex(
                name: "IX_Socio_GimnasioId",
                table: "Socio");

            migrationBuilder.DropColumn(
                name: "GimnasioId",
                table: "SocioRealizaLogro");

            migrationBuilder.DropColumn(
                name: "GimnasioId",
                table: "Socio");
        }
    }
}
