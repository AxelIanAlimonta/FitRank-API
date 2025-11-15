using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionAmistad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amistades_Socios_SolicitanteId1",
                table: "Amistades");

            migrationBuilder.DropIndex(
                name: "IX_Amistades_SolicitanteId1",
                table: "Amistades");

            migrationBuilder.DropColumn(
                name: "SolicitanteId1",
                table: "Amistades");

            migrationBuilder.AlterColumn<long>(
                name: "SolicitanteId",
                table: "Amistades",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "SocioId2",
                table: "Amistades",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "SocioId1",
                table: "Amistades",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Amistades",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Amistades_SolicitanteId",
                table: "Amistades",
                column: "SolicitanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amistades_Socios_SolicitanteId",
                table: "Amistades",
                column: "SolicitanteId",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amistades_Socios_SolicitanteId",
                table: "Amistades");

            migrationBuilder.DropIndex(
                name: "IX_Amistades_SolicitanteId",
                table: "Amistades");

            migrationBuilder.AlterColumn<int>(
                name: "SolicitanteId",
                table: "Amistades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "SocioId2",
                table: "Amistades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "SocioId1",
                table: "Amistades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Amistades",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "SolicitanteId1",
                table: "Amistades",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Amistades_SolicitanteId1",
                table: "Amistades",
                column: "SolicitanteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Amistades_Socios_SolicitanteId1",
                table: "Amistades",
                column: "SolicitanteId1",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
