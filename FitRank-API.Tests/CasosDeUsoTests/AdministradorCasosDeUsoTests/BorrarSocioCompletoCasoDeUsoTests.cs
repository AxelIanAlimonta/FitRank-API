using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Tests.CasosDeUsoTests.AdministradorCasosDeUsoTests
{
    public class BorrarSocioCompletoCasoDeUsoTests
    {
        private readonly FitRankDbContext _context;
        private readonly BorrarSocioCompletoCasoDeUso _casoDeUso;

        public BorrarSocioCompletoCasoDeUsoTests()
        {
            var options = new DbContextOptionsBuilder<FitRankDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FitRankDbContext(options);
            _casoDeUso = new BorrarSocioCompletoCasoDeUso(_context);
        }

        [Fact]
        public async Task Ejecutar_DebeEliminarSocioConTodosSusDatos()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Email = "socio@test.com",
                Nombre = "Test",
                Apellido = "Socio",
                Rol = "Socio",
                Nivel = "Principiante"
            };

            _context.Socios.Add(socio);
            _context.SaveChanges();

            var asistencia = new Asistencia { Id = 1, UsuarioId = 1, Fecha = DateTime.UtcNow, Observaciones = string.Empty };
            _context.Asistencias.Add(asistencia);

            var invitacion = new Invitacion { Id = 1, UsuarioId = 1, Email = "test@test.com" };
            _context.Invitaciones.Add(invitacion);

            await _context.SaveChangesAsync();

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Contain("eliminado completamente");
            _context.Socios.Should().BeEmpty();
            _context.Asistencias.Should().BeEmpty();
            _context.Invitaciones.Where(i => i.UsuarioId == 1).Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeEliminarSocioAunqueSoloPorUsuarioId()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 5,
                Email = "usuario@test.com",
                Rol = "User",
                PasswordHash = "hash"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().Contain("eliminado");
            _context.Usuarios.Find(5L).Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarMensajeExitoso()
        {
            // Arrange
            var socio = new Socio { Id = 10, Email = "test@test.com", Rol = "Socio", Nivel = "Intermedio" };
            _context.Socios.Add(socio);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _casoDeUso.Ejecutar(10);

            // Assert
            resultado.Should().NotBeNullOrEmpty();
            resultado.Should().Contain("Usuario 10 eliminado completamente");
        }

        [Fact]
        public async Task Ejecutar_DebeEliminarVariasAsistenciasDelMismoUsuario()
        {
            // Arrange
            var socio = new Socio { Id = 20, Email = "socio@test.com", Rol = "Socio", Nivel = "Avanzado" };
            _context.Socios.Add(socio);

            _context.Asistencias.AddRange(
                new Asistencia { Id = 1, UsuarioId = 20, Fecha = DateTime.UtcNow, Observaciones = string.Empty },
                new Asistencia { Id = 2, UsuarioId = 20, Fecha = DateTime.UtcNow.AddDays(-1), Observaciones = string.Empty },
                new Asistencia { Id = 3, UsuarioId = 20, Fecha = DateTime.UtcNow.AddDays(-2), Observaciones = string.Empty }
            );

            await _context.SaveChangesAsync();

            // Act
            await _casoDeUso.Ejecutar(20);

            // Assert
            _context.Asistencias.Where(a => a.UsuarioId == 20).Should().BeEmpty();
        }
    }
}
