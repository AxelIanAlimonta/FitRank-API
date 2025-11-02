using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class CambiarDuracionASpan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cambia de DateTime a TimeSpan (interval)
            migrationBuilder.Sql(
                "ALTER TABLE \"Series\" ALTER COLUMN \"Duracion\" TYPE interval USING \"Duracion\"::interval;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Cambia de TimeSpan (interval) a DateTime
            migrationBuilder.Sql(
                "ALTER TABLE \"Series\" ALTER COLUMN \"Duracion\" TYPE timestamp with time zone USING \"Duracion\"::timestamp;");
        }
    }
}
