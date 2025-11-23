using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Tests.InfrastructureTests;

public class SolicitudRutinaProfesorRepositorioImplTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }
    private async Task<(Socio, Profesor, Rutina)> SeedData(FitRankDbContext context)
    {
        var socio = new Socio
        {
            Nombre = "Test Socio",
            Apellido = "Apellido",
            Email = "testmail@gmail.com",
            Nivel = "Intermedio",
        };

        var profesor = new Profesor
        {
            Nombre = "Test Profesor",
            Apellido = "Apellido",
            Email = "",
            Matricula = "MAT123",
            Sueldo = 50000
        };

        var rutina = new Rutina
        {
            Nombre = "Rutina Test",
            TipoCreacion = "Automatica",
            FechaCreacion = DateTime.UtcNow,
            Activa = true,
            Favorita = false,
            Socio = socio,
            Usuario = profesor,
            Valoraciones = new List<Valoracion>
            {
                new Valoracion { Puntaje = 5 },
                new Valoracion { Puntaje = 10 },
                new Valoracion { Puntaje = 15 }
            }
        };


        context.Socios.Add(socio);
        context.Profesores.Add(profesor);
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();
        return (socio, profesor, rutina);
    }

    //Agregar SolicitudRutinaProfesor debe agregar una solicitud correctamente
    [Fact]
    public async Task AgregarSolicitudRutinaProfesor_DebeAgregarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);

        var solicitud = new SolicitudRutinaProfesor
        {
            SocioId = 1,
            NombreSocio = "Juan Perez",
            Edad = 25,
            PesoKg = 70,
            AlturaCm = 175,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder peso",
            CalidadAlimentacion = 3,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 70
        };
        // Act
        await repositorio.AgregarAsync(solicitud);

        // FluentAssert
        var solicitudEnDb = context.SolicitudesRutinaProfesor.FirstOrDefault(s => s.SocioId == 1);
        solicitudEnDb.Should().NotBeNull();
        solicitudEnDb!.NombreSocio.Should().Be("Juan Perez");
        solicitudEnDb.Edad.Should().Be(25);

    }

    //obtener por id debe retornar la solicitud correcta
    [Fact]
    public async Task ObtenerPorId_DebeRetornarSolicitudCorrecta()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio, profesor, rutina) = await SeedData(context);
        var solicitud = new SolicitudRutinaProfesor
        {
            SocioId = socio.Id,
            NombreSocio = "Maria Gomez",
            Edad = 30,
            PesoKg = 65,
            AlturaCm = 165,
            Nivel = "Principiante",
            SesionesPorSemana = 3,
            MinutosPorSesion = 45,
            Objetivo = "Ganar masa muscular",
            CalidadAlimentacion = 4,
            HorasSuenio = 8,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 75,
            ProfesorId = profesor.Id,
            RutinaId = rutina.Id
        };
        context.SolicitudesRutinaProfesor.Add(solicitud);
        await context.SaveChangesAsync();
        // Act
        var solicitudObtenida = await repositorio.ObtenerPorIdAsync(solicitud.Id);
        // FluentAssert
        solicitudObtenida.Should().NotBeNull();
        solicitudObtenida!.NombreSocio.Should().Be("Maria Gomez");
    }

    // obtener lista de pendientes debe retornar solo las solicitudes pendientes
    [Fact]
    public async Task ObtenerPendientes_DebeRetornarSoloSolicitudesPendientes()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPendientesSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = 3,
            NombreSocio = "Carlos Lopez",
            Estado = EstadoSolicitud.Pendiente,
            Edad = 28,
            PesoKg = 80,
            AlturaCm = 180,
            Nivel = "Avanzado",
            SesionesPorSemana = 5,
            MinutosPorSesion = 70,
            Objetivo = "Mejorar resistencia",
            CalidadAlimentacion = 5,
            HorasSuenio = 6,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 65
        };
        var solicitud2 = new SolicitudRutinaProfesor
        {
            SocioId = 4,
            NombreSocio = "Ana Martinez",
            Estado = EstadoSolicitud.Finalizada,
            Edad = 22,
            PesoKg = 55,
            AlturaCm = 160,
            Nivel = "Principiante",
            SesionesPorSemana = 2,
            MinutosPorSesion = 30,
            Objetivo = "Tonificar",
            CalidadAlimentacion = 4,
            HorasSuenio = 8,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 72
        };
        context.SolicitudesRutinaProfesor.AddRange(solicitud1, solicitud2);
        await context.SaveChangesAsync();
        // Act
        var pendientes = await repositorio.ObtenerPendientesAsync();
        // FluentAssert
        pendientes.Should().HaveCount(1);
        pendientes[0].NombreSocio.Should().Be("Carlos Lopez");

    }

    //obtener por profesor id debe retornar las solicitudes correctas
    [Fact]
    public async Task ObtenerPorProfesorId_DebeRetornarSolicitudesCorrectas()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorProfesorIdSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio, profesor, _) = await SeedData(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = socio.Id,
            NombreSocio = $"{socio.Nombre} {socio.Apellido}",
            ProfesorId = profesor.Id,
            Edad = 35,
            PesoKg = 90,
            AlturaCm = 185,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 3,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 68
        };
        context.SolicitudesRutinaProfesor.Add(solicitud1);
        await context.SaveChangesAsync();
        // Act
        var solicitudesDelProfesor = await repositorio.ObtenerPorProfesorAsync(profesor.Id);
        // FluentAssert
        solicitudesDelProfesor.Should().HaveCount(1);
        solicitudesDelProfesor[0].NombreSocio.Should().Be($"{socio.Nombre} {socio.Apellido}");
    }

    //obtener por socio id debe retornar las solicitudes correctas
    [Fact]
    public async Task ObtenerPorSocioId_DebeRetornarSolicitudesCorrectas()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorSocioIdSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio, profesor, rutina) = await SeedData(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = socio.Id,
            NombreSocio = $"{socio.Nombre} {socio.Apellido}",
            Edad = 29,
            PesoKg = 60,
            AlturaCm = 170,
            Nivel = "Avanzado",
            SesionesPorSemana = 5,
            MinutosPorSesion = 75,
            Objetivo = "Aumentar fuerza",
            CalidadAlimentacion = 5,
            HorasSuenio = 6,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 64,
            ProfesorId = profesor.Id,
            RutinaId = rutina.Id
        };
        context.SolicitudesRutinaProfesor.Add(solicitud1);
        await context.SaveChangesAsync();
        // Act
        var solicitudesDelSocio = await repositorio.ObtenerPorSocioAsync(socio.Id);
        // FluentAssert
        solicitudesDelSocio.Should().HaveCount(1);
        solicitudesDelSocio[0].NombreSocio.Should().Be($"{socio.Nombre} {socio.Apellido}");
    }

    //actializar solicitud debe guardar los cambios correctamente
    [Fact]
    public async Task ActualizarSolicitud_DebeGuardarCambiosCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var solicitud = new SolicitudRutinaProfesor
        {
            SocioId = 5,
            NombreSocio = "Luis Fernandez",
            Estado = EstadoSolicitud.Pendiente,
            Edad = 32,
            PesoKg = 75,
            AlturaCm = 178,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Mejorar condicion fisica",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 70
        };
        context.SolicitudesRutinaProfesor.Add(solicitud);
        await context.SaveChangesAsync();
        var solicitudActualizada = new SolicitudRutinaProfesor
        {
            Id = solicitud.Id,
            SocioId = 5,
            NombreSocio = "Luis Fernandez",
            Estado = EstadoSolicitud.Finalizada,
            Edad = 32,
            PesoKg = 75,
            AlturaCm = 178,
            Nivel = "Avanzado",
            SesionesPorSemana = 5,
            MinutosPorSesion = 70,
            Objetivo = "Mejorar condicion fisica actualizada",
            CalidadAlimentacion = 3,
            HorasSuenio = 8,
            DolorLumbar = true,
            DolorRodilla = true,
            DolorHombro = true,
            CirugiaReciente = true,
            Sincope = true,
            Embarazo = true,
            Hipertension = true,
            HipertensionControlada = true,
            Diabetes = true,
            DolorToracico = true,
            FrecuenciaCardiacaReposo = 80
        };

        // Act
        await repositorio.ActualizarAsync(solicitudActualizada);
        // FluentAssert
        var solicitudEnDb = await context.SolicitudesRutinaProfesor.FindAsync(solicitud.Id);
        //verificar todos los cambios
        solicitudEnDb!.Estado.Should().Be(EstadoSolicitud.Finalizada);
        solicitudEnDb.Nivel.Should().Be("Avanzado");
        solicitudEnDb.SesionesPorSemana.Should().Be(5);
        solicitudEnDb.MinutosPorSesion.Should().Be(70);
        solicitudEnDb.Objetivo.Should().Be("Mejorar condicion fisica actualizada");
        solicitudEnDb.CalidadAlimentacion.Should().Be(3);
        solicitudEnDb.HorasSuenio.Should().Be(8);
        solicitudEnDb.DolorLumbar.Should().BeTrue();
        solicitudEnDb.DolorRodilla.Should().BeTrue();
        solicitudEnDb.DolorHombro.Should().BeTrue();
        solicitudEnDb.CirugiaReciente.Should().BeTrue();
        solicitudEnDb.Sincope.Should().BeTrue();
        solicitudEnDb.Embarazo.Should().BeTrue();
        solicitudEnDb.Hipertension.Should().BeTrue();
        solicitudEnDb.HipertensionControlada.Should().BeTrue();
        solicitudEnDb.Diabetes.Should().BeTrue();
        solicitudEnDb.DolorToracico.Should().BeTrue();
        solicitudEnDb.FrecuenciaCardiacaReposo.Should().Be(80);
    }


    //obtener profesor mas solicitado
    [Fact]
    public async Task ObtenerProfesorMasSolicitado_DevuelveElMasSolicitado()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEstadisticasProfesoresSolicitudRutinaProfesorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio1, profesor1, _) = await SeedData(context);
        var (_, profesor2, _) = await SeedData(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor1.Id,
            Estado = EstadoSolicitud.Finalizada,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        var solicitud2 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor2.Id,
            Estado = EstadoSolicitud.Pendiente,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        var solicitud3 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor1.Id,
            Estado = EstadoSolicitud.Finalizada,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };

        context.SolicitudesRutinaProfesor.AddRange(solicitud1, solicitud2, solicitud3);
        await context.SaveChangesAsync();
        // Act
        var profesorMasSolicitado = await repositorio.ObtenerProfesorMasSolicitadoAsync();
        // FluentAssert
        profesorMasSolicitado.Should().NotBeNull();
        profesorMasSolicitado!.Id.Should().Be(profesor1.Id);
    }

    //obtener profesor mas solicitado cuando no hay solicitudes debe retornar null
    [Fact]
    public async Task ObtenerProfesorMasSolicitado_SinSolicitudes_DebeRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorMasSolicitado_SinSolicitudesDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        // Act
        var profesorMasSolicitado = await repositorio.ObtenerProfesorMasSolicitadoAsync();
        // FluentAssert
        profesorMasSolicitado.Should().BeNull();
    }

    //obtener el profesor con mas solicitudes pendientes
    [Fact]
    public async Task ObtenerProfesorConMasSolicitudesPendientes_DevuelveElCorrecto()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorConMasSolicitudesPendientesDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio1, profesor1, _) = await SeedData(context);
        var (_, profesor2, _) = await SeedData(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor1.Id,
            Estado = EstadoSolicitud.Pendiente,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        var solicitud2 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor2.Id,
            Estado = EstadoSolicitud.Pendiente,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        var solicitud3 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor2.Id,
            Estado = EstadoSolicitud.Pendiente,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        context.SolicitudesRutinaProfesor.AddRange(solicitud1, solicitud2, solicitud3);
        await context.SaveChangesAsync();
        // Act
        var profesorConMasPendientes = await repositorio.ObtenerProfesorConMasPendientesAsync();

        // FluentAssert
        profesorConMasPendientes.Should().NotBeNull();
        profesorConMasPendientes!.Id.Should().Be(profesor2.Id);
    }

    //obtener el profesor con mas solicitudes pendientes cuando no hay solicitudes debe retornar null
    [Fact]
    public async Task ObtenerProfesorConMasSolicitudesPendientes_SinSolicitudes_DebeRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorConMasSolicitudesPendientes_SinSolicitudesDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        // Act
        var profesorConMasPendientes = await repositorio.ObtenerProfesorConMasPendientesAsync();
        // FluentAssert
        profesorConMasPendientes.Should().BeNull();
    }

    //obtener profesor mas cumplidor
    [Fact]
    public async Task ObtenerProfesorMasCumplidor_DevuelveElMasCumplidor()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorMasCumplidorDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio1, profesor1, _) = await SeedData(context);
        var (_, profesor2, _) = await SeedData(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor1.Id,
            Estado = EstadoSolicitud.Finalizada,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        var solicitud2 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor2.Id,
            Estado = EstadoSolicitud.TomadaPorProfesor,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        var solicitud3 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            ProfesorId = profesor2.Id,
            Estado = EstadoSolicitud.Finalizada,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        context.SolicitudesRutinaProfesor.AddRange(solicitud1, solicitud2, solicitud3);
        await context.SaveChangesAsync();
        // Act
        var profesorMasCumplidor = await repositorio.ObtenerProfesorMasCumplidorAsync();
        // FluentAssert
        profesorMasCumplidor.Should().NotBeNull();
        profesorMasCumplidor!.Id.Should().Be(profesor2.Id);
    }

    //obtener profesor mas cumplidor cuando no hay solicitudes debe retornar null
    [Fact]
    public async Task ObtenerProfesorMasCumplidor_SinSolicitudes_DebeRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorMasCumplidor_SinSolicitudesDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        // Act
        var profesorMasCumplidor = await repositorio.ObtenerProfesorMasCumplidorAsync();
        // FluentAssert
        profesorMasCumplidor.Should().BeNull();
    }

    //obtener profesor mejor promedio valoraciones y el valor del promedio
    [Fact]
    public async Task ObtenerProfesorMejorPromedioValoraciones_DevuelveElCorrectoYPromedio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorMejorPromedioValoracionesDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        var (socio1, profesor1, rutina1) = await SeedData(context);
        var solicitud1 = new SolicitudRutinaProfesor
        {
            SocioId = socio1.Id,
            NombreSocio = $"{socio1.Nombre} {socio1.Apellido}",
            RutinaId = rutina1.Id,
            ProfesorId = profesor1.Id,
            Estado = EstadoSolicitud.Finalizada,
            Edad = 27,
            PesoKg = 68,
            AlturaCm = 172,
            Nivel = "Intermedio",
            SesionesPorSemana = 4,
            MinutosPorSesion = 60,
            Objetivo = "Perder grasa",
            CalidadAlimentacion = 4,
            HorasSuenio = 7,
            DolorLumbar = false,
            DolorRodilla = false,
            DolorHombro = false,
            CirugiaReciente = false,
            Sincope = false,
            Embarazo = false,
            Hipertension = false,
            HipertensionControlada = false,
            Diabetes = false,
            DolorToracico = false,
            FrecuenciaCardiacaReposo = 66
        };
        context.SolicitudesRutinaProfesor.AddRange(solicitud1);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerProfesorMejorPromedioValoracionesAsync();
        var profesorMejorPromedio = resultado?.Item1;
        var promedio = resultado?.Item2;
        // FluentAssert
        profesorMejorPromedio.Should().NotBeNull();
        profesorMejorPromedio!.Id.Should().Be(profesor1.Id);
        promedio.Should().BeGreaterThan(0);
        promedio.Should().BeApproximately(10.0, 0.1); // El promedio de las valoraciones es (5+10+15)/3 = 10
    }

    //obtener profesor mejor promedio valoraciones cuando no hay solicitudes debe retornar null y null
    [Fact]
    public async Task ObtenerProfesorMejorPromedioValoraciones_SinSolicitudes_DebeRetornarNullYNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerProfesorMejorPromedioValoraciones_SinSolicitudesDB");
        using var context = new FitRankDbContext(options);
        var repositorio = new SolicitudRutinaProfesorRepositorioImpl(context);
        // Act
        var resultado = await repositorio.ObtenerProfesorMejorPromedioValoracionesAsync();
        var profesorMejorPromedio = resultado?.Item1;
        var promedio = resultado?.Item2;
        // FluentAssert
        profesorMejorPromedio.Should().BeNull();
        promedio.Should().BeNull();
    }

}
