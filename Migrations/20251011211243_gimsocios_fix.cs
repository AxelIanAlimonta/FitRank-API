using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitRank_API.Migrations
{
    public partial class gimsocios_fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 0) (Opcional) Semilla de un gimnasio por si está vacío
            migrationBuilder.Sql(@"
                INSERT INTO ""Gimnasios"" (""Nombre"",""Direccion"",""Telefono"",""Email"")
                SELECT 'Gimnasio General','-','-','-'
                WHERE NOT EXISTS (SELECT 1 FROM ""Gimnasios"");
            ");

            // 1) SOCIO.GimnasioId  (ADD COLUMN IF NOT EXISTS -> backfill -> NOT NULL -> IDX + FK)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Socio"" ADD COLUMN IF NOT EXISTS ""GimnasioId"" integer;
                UPDATE ""Socio""
                   SET ""GimnasioId"" = (SELECT ""Id"" FROM ""Gimnasios"" ORDER BY ""Id"" LIMIT 1)
                 WHERE ""GimnasioId"" IS NULL;
                ALTER TABLE ""Socio"" ALTER COLUMN ""GimnasioId"" SET NOT NULL;

                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE tablename = 'Socio' AND indexname = 'IX_Socio_GimnasioId'
                    ) THEN
                        CREATE INDEX ""IX_Socio_GimnasioId"" ON ""Socio"" (""GimnasioId"");
                    END IF;
                END $$;

                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint 
                        WHERE conname = 'FK_Socio_Gimnasios_GimnasioId'
                    ) THEN
                        ALTER TABLE ""Socio""
                          ADD CONSTRAINT ""FK_Socio_Gimnasios_GimnasioId""
                          FOREIGN KEY (""GimnasioId"") REFERENCES ""Gimnasios""(""Id"") ON DELETE RESTRICT;
                    END IF;
                END $$;
            ");

            // 2) SOCIOREALIZALOGRO.GimnasioId  (ADD COLUMN IF NOT EXISTS -> backfill -> NOT NULL -> IDX + FK + UNIQUE)
            migrationBuilder.Sql(@"
                ALTER TABLE ""SocioRealizaLogro"" ADD COLUMN IF NOT EXISTS ""GimnasioId"" integer;

                -- backfill desde el socio
                UPDATE ""SocioRealizaLogro"" srl
                   SET ""GimnasioId"" = s.""GimnasioId""
                  FROM ""Socio"" s
                 WHERE srl.""SocioId"" = s.""Id"" AND srl.""GimnasioId"" IS NULL;

                -- fallback por si quedó null
                UPDATE ""SocioRealizaLogro""
                   SET ""GimnasioId"" = (SELECT ""Id"" FROM ""Gimnasios"" ORDER BY ""Id"" LIMIT 1)
                 WHERE ""GimnasioId"" IS NULL;

                ALTER TABLE ""SocioRealizaLogro"" ALTER COLUMN ""GimnasioId"" SET NOT NULL;

                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE tablename = 'SocioRealizaLogro' AND indexname = 'IX_SocioRealizaLogro_GimnasioId'
                    ) THEN
                        CREATE INDEX ""IX_SocioRealizaLogro_GimnasioId"" ON ""SocioRealizaLogro"" (""GimnasioId"");
                    END IF;
                END $$;

                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint 
                        WHERE conname = 'FK_SocioRealizaLogro_Gimnasios_GimnasioId'
                    ) THEN
                        ALTER TABLE ""SocioRealizaLogro""
                          ADD CONSTRAINT ""FK_SocioRealizaLogro_Gimnasios_GimnasioId""
                          FOREIGN KEY (""GimnasioId"") REFERENCES ""Gimnasios""(""Id"") ON DELETE RESTRICT;
                    END IF;
                END $$;

                -- Reemplazar UNIQUE (SocioId, LogroId) por (SocioId, LogroId, GimnasioId)
                DO $$
                DECLARE idx text;
                BEGIN
                    SELECT indexname INTO idx
                    FROM pg_indexes
                    WHERE tablename = 'SocioRealizaLogro'
                      AND indexname ILIKE 'IX_SocioRealizaLogro_SocioId_LogroId';
                    IF idx IS NOT NULL THEN
                        EXECUTE format('DROP INDEX ""%s"";', idx);
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE tablename = 'SocioRealizaLogro' 
                          AND indexname = 'IX_SocioRealizaLogro_SocioId_LogroId_GimnasioId'
                    ) THEN
                        CREATE UNIQUE INDEX ""IX_SocioRealizaLogro_SocioId_LogroId_GimnasioId""
                        ON ""SocioRealizaLogro"" (""SocioId"",""LogroId"",""GimnasioId"");
                    END IF;
                END $$;
            ");

            // 3) TABLA intermedia GIMNASIOLOGRO (IF NOT EXISTS)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM information_schema.tables 
                        WHERE table_schema='public' AND table_name='GimnasioLogro'
                    ) THEN
                        CREATE TABLE ""GimnasioLogro"" (
                            ""Id""          integer GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                            ""GimnasioId""  integer NOT NULL,
                            ""LogroId""     integer NOT NULL,
                            ""Activo""      boolean NOT NULL DEFAULT TRUE,
                            CONSTRAINT ""FK_GimnasioLogro_Gimnasios_GimnasioId""
                              FOREIGN KEY (""GimnasioId"") REFERENCES ""Gimnasios""(""Id"") ON DELETE CASCADE,
                            CONSTRAINT ""FK_GimnasioLogro_Logro_LogroId""
                              FOREIGN KEY (""LogroId"") REFERENCES ""Logro""(""Id"") ON DELETE RESTRICT,
                            CONSTRAINT ""UQ_GimnasioLogro_GimnasioId_LogroId"" UNIQUE (""GimnasioId"",""LogroId"")
                        );
                        CREATE INDEX ""IX_GimnasioLogro_GimnasioId"" ON ""GimnasioLogro"" (""GimnasioId"");
                        CREATE INDEX ""IX_GimnasioLogro_LogroId"" ON ""GimnasioLogro"" (""LogroId"");
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // borrar tabla intermedia si existe
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""GimnasioLogro"";");

            // revertir columnas/índices/constraints de SocioRealizaLogro
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname='FK_SocioRealizaLogro_Gimnasios_GimnasioId') THEN
                        ALTER TABLE ""SocioRealizaLogro"" DROP CONSTRAINT ""FK_SocioRealizaLogro_Gimnasios_GimnasioId"";
                    END IF;
                END $$;
                DROP INDEX IF EXISTS ""IX_SocioRealizaLogro_SocioId_LogroId_GimnasioId"";
                DROP INDEX IF EXISTS ""IX_SocioRealizaLogro_GimnasioId"";
                ALTER TABLE ""SocioRealizaLogro"" DROP COLUMN IF EXISTS ""GimnasioId"";
            ");

            // revertir columnas/índices/constraints de Socio
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname='FK_Socio_Gimnasios_GimnasioId') THEN
                        ALTER TABLE ""Socio"" DROP CONSTRAINT ""FK_Socio_Gimnasios_GimnasioId"";
                    END IF;
                END $$;
                DROP INDEX IF EXISTS ""IX_Socio_GimnasioId"";
                ALTER TABLE ""Socio"" DROP COLUMN IF EXISTS ""GimnasioId"";
            ");

            // NO se elimina ""Gimnasios"" porque ya existía antes de esta migration
        }
    }
}
