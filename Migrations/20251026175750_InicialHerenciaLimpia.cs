using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitRank_API.Migrations
{
    /// <inheritdoc />
    public partial class InicialHerenciaLimpia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dificultades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dificultades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gimnasio",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    RazonSocial = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    ColorPrincipal = table.Column<string>(type: "text", nullable: false),
                    ColorSecundario = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: false),
                    Cuil = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gimnasio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GruposMusculares",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposMusculares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logros",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Imagen = table.Column<string>(type: "text", nullable: false),
                    Global = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Edad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SesionRealizadaDeEjercicios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duracion = table.Column<TimeSpan>(type: "interval", nullable: false),
                    NumeroDeSesion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionRealizadaDeEjercicios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Dni = table.Column<int>(type: "integer", nullable: false),
                    NombreUsuario = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Rol = table.Column<string>(type: "text", nullable: false),
                    Sexo = table.Column<string>(type: "text", nullable: false),
                    QrToken = table.Column<string>(type: "text", nullable: true),
                    TokenRecuperacion = table.Column<string>(type: "text", nullable: true),
                    TokenExpira = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FotoDePerfil = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CuotaPagadaHasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    EsActivado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rutinas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Frecuencia = table.Column<int>(type: "integer", nullable: true),
                    DificultadId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rutinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rutinas_Dificultades_DificultadId",
                        column: x => x.DificultadId,
                        principalTable: "Dificultades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesGrupoMuscular",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Multiplicadopeso = table.Column<double>(type: "double precision", nullable: false),
                    MultiplicadorRepeticiones = table.Column<double>(type: "double precision", nullable: false),
                    GrupoMuscularId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesGrupoMuscular", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesGrupoMuscular_GruposMusculares_GrupoMuscular~",
                        column: x => x.GrupoMuscularId,
                        principalTable: "GruposMusculares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ejercicios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    UrlVideo = table.Column<string>(type: "text", nullable: false),
                    GrupoMuscularId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejercicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ejercicios_GruposMusculares_GrupoMuscularId",
                        column: x => x.GrupoMuscularId,
                        principalTable: "GruposMusculares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Socios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    GimnasioId = table.Column<long>(type: "bigint", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Altura = table.Column<double>(type: "double precision", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    Nivel = table.Column<string>(type: "text", nullable: false),
                    Puntaje = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Socios_Gimnasio_GimnasioId",
                        column: x => x.GimnasioId,
                        principalTable: "Gimnasio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Socios_Usuarios_Id",
                        column: x => x.Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RutinasEjercicios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RutinaId = table.Column<long>(type: "bigint", nullable: false),
                    EjercicioId = table.Column<long>(type: "bigint", nullable: false),
                    NumeroDeSesion = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutinasEjercicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RutinasEjercicios_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RutinasEjercicios_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Presente = table.Column<bool>(type: "boolean", nullable: false),
                    HoraEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraSalida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: false),
                    GimnasioId = table.Column<long>(type: "bigint", nullable: false),
                    SocioId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencias_Gimnasio_GimnasioId",
                        column: x => x.GimnasioId,
                        principalTable: "Gimnasio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencias_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asistencias_Socios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EjerciciosAsignados",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Orden = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Sesion = table.Column<int>(type: "integer", nullable: true),
                    RutinaId = table.Column<long>(type: "bigint", nullable: false),
                    EjercicioId = table.Column<long>(type: "bigint", nullable: false),
                    SocioId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EjerciciosAsignados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EjerciciosAsignados_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjerciciosAsignados_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjerciciosAsignados_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EjerciciosRealizados",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EjercicioId = table.Column<long>(type: "bigint", nullable: false),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    RutinaId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EjerciciosRealizados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EjerciciosRealizados_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjerciciosRealizados_Rutinas_RutinaId",
                        column: x => x.RutinaId,
                        principalTable: "Rutinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EjerciciosRealizados_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GimnasioId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    DatosPrellenados = table.Column<string>(type: "text", nullable: false),
                    MetodoPago = table.Column<string>(type: "text", nullable: false),
                    MpPaymentId = table.Column<string>(type: "text", nullable: true),
                    CreadaEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiraEn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<long>(type: "bigint", nullable: true),
                    CuotaPagadaHasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Gimnasio_GimnasioId",
                        column: x => x.GimnasioId,
                        principalTable: "Gimnasio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invitaciones_Socios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Puntajes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SocioId = table.Column<long>(type: "bigint", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Valor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puntajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puntajes_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesAsignadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Peso = table.Column<int>(type: "integer", nullable: false),
                    Repeticiones = table.Column<int>(type: "integer", nullable: false),
                    Rir = table.Column<int>(type: "integer", nullable: false),
                    NroSerie = table.Column<int>(type: "integer", nullable: false),
                    EjercicioAsignadoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesAsignadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesAsignadas_EjerciciosAsignados_EjercicioAsignadoId",
                        column: x => x.EjercicioAsignadoId,
                        principalTable: "EjerciciosAsignados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeriesRealizadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Repeticiones = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    Rir = table.Column<int>(type: "integer", nullable: false),
                    NumeroDeSerie = table.Column<int>(type: "integer", nullable: false),
                    EjercicioRealizadoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeriesRealizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeriesRealizadas_EjerciciosRealizados_EjercicioRealizadoId",
                        column: x => x.EjercicioRealizadoId,
                        principalTable: "EjerciciosRealizados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_GimnasioId",
                table: "Asistencias",
                column: "GimnasioId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_SocioId",
                table: "Asistencias",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioId",
                table: "Asistencias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesGrupoMuscular_GrupoMuscularId",
                table: "ConfiguracionesGrupoMuscular",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_Ejercicios_GrupoMuscularId",
                table: "Ejercicios",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosAsignados_EjercicioId",
                table: "EjerciciosAsignados",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosAsignados_RutinaId",
                table: "EjerciciosAsignados",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosAsignados_SocioId",
                table: "EjerciciosAsignados",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_EjercicioId",
                table: "EjerciciosRealizados",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_RutinaId",
                table: "EjerciciosRealizados",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_EjerciciosRealizados_SocioId",
                table: "EjerciciosRealizados",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_GimnasioId",
                table: "Invitaciones",
                column: "GimnasioId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_UsuarioId",
                table: "Invitaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Puntajes_SocioId",
                table: "Puntajes",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_DificultadId",
                table: "Rutinas",
                column: "DificultadId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasEjercicios_EjercicioId",
                table: "RutinasEjercicios",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinasEjercicios_RutinaId",
                table: "RutinasEjercicios",
                column: "RutinaId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesAsignadas_EjercicioAsignadoId",
                table: "SeriesAsignadas",
                column: "EjercicioAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeriesRealizadas_EjercicioRealizadoId",
                table: "SeriesRealizadas",
                column: "EjercicioRealizadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Socios_GimnasioId",
                table: "Socios",
                column: "GimnasioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "ConfiguracionesGrupoMuscular");

            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropTable(
                name: "Logros");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Puntajes");

            migrationBuilder.DropTable(
                name: "RutinasEjercicios");

            migrationBuilder.DropTable(
                name: "SeriesAsignadas");

            migrationBuilder.DropTable(
                name: "SeriesRealizadas");

            migrationBuilder.DropTable(
                name: "SesionRealizadaDeEjercicios");

            migrationBuilder.DropTable(
                name: "EjerciciosAsignados");

            migrationBuilder.DropTable(
                name: "EjerciciosRealizados");

            migrationBuilder.DropTable(
                name: "Ejercicios");

            migrationBuilder.DropTable(
                name: "Rutinas");

            migrationBuilder.DropTable(
                name: "Socios");

            migrationBuilder.DropTable(
                name: "GruposMusculares");

            migrationBuilder.DropTable(
                name: "Dificultades");

            migrationBuilder.DropTable(
                name: "Gimnasio");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
