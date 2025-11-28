using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SolicitudRutinaProfesorRepositorioImpl : ISolicitudRutinaProfesorRepositorio
    {
        private readonly FitRankDbContext _context;

        public SolicitudRutinaProfesorRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(SolicitudRutinaProfesor solicitud)
        {
            _context.SolicitudesRutinaProfesor.Add(solicitud);
            await _context.SaveChangesAsync();
        }

        public async Task<SolicitudRutinaProfesor?> ObtenerPorIdAsync(long id)
        {
            return await _context.SolicitudesRutinaProfesor
                .Include(s => s.Socio)
                .Include(s => s.Profesor)
                .Include(s => s.Rutina)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SolicitudRutinaProfesorDTO>> ObtenerPendientesAsync()
        {
            return await _context.SolicitudesRutinaProfesor
                .Where(s => s.Estado == EstadoSolicitud.Pendiente)
                .Include(s => s.Socio)
                .Select(s => new SolicitudRutinaProfesorDTO
                {
                    Id = s.Id,
                    SocioId = s.SocioId,
                    NombreSocio = s.NombreSocio,
                    ProfesorId = s.ProfesorId,
                    Estado = s.Estado.ToString(),
                    FechaSolicitud = s.FechaSolicitud,
                    MensajeSocio = s.MensajeSocio,
                    Edad = s.Edad,
                    PesoKg = s.PesoKg,
                    AlturaCm = s.AlturaCm,
                    Nivel = s.Nivel,
                    SesionesPorSemana = s.SesionesPorSemana,
                    MinutosPorSesion = s.MinutosPorSesion,
                    Objetivo = s.Objetivo,

                    DolorLumbar = s.DolorLumbar,
                    DolorRodilla = s.DolorRodilla,
                    DolorHombro = s.DolorHombro,
                    CirugiaReciente = s.CirugiaReciente,
                    Sincope = s.Sincope,
                    Embarazo = s.Embarazo,
                    Hipertension = s.Hipertension,
                    HipertensionControlada = s.HipertensionControlada,
                    Diabetes = s.Diabetes,
                    DolorToracico = s.DolorToracico
                })
                .ToListAsync();
        }

        public async Task<List<SolicitudRutinaProfesorDTO>> ObtenerPorProfesorAsync(long profesorId)
        {
            return await _context.SolicitudesRutinaProfesor
                .Where(s => s.ProfesorId == profesorId)
                .Include(s => s.Socio)
                .Select(s => new SolicitudRutinaProfesorDTO
                {
                    Id = s.Id,
                    SocioId = s.SocioId,
                    NombreSocio = $"{s.Socio.Nombre} {s.Socio.Apellido}",
                    ProfesorId = s.ProfesorId,
                    Estado = s.Estado.ToString(),
                    FechaSolicitud = s.FechaSolicitud,
                    MensajeSocio = s.MensajeSocio,
                    MensajeProfesor = s.MensajeProfesor,
                    RutinaId = s.RutinaId,
                    Edad = s.Edad,
                    PesoKg = s.PesoKg,
                    AlturaCm = s.AlturaCm,
                    Nivel = s.Nivel,
                    SesionesPorSemana = s.SesionesPorSemana,
                    MinutosPorSesion = s.MinutosPorSesion,
                    Objetivo = s.Objetivo
                })
                .ToListAsync();
        }

        public async Task<List<SolicitudRutinaProfesorDTO>> ObtenerPorSocioAsync(long socioId)
        {
            return await _context.SolicitudesRutinaProfesor
                .Where(s => s.SocioId == socioId)
                .Include(s => s.Profesor)
                .Select(s => new SolicitudRutinaProfesorDTO
                {
                    Id = s.Id,
                    SocioId = s.SocioId,
                    NombreSocio = s.NombreSocio, // <-- Agrega esta línea
                    NombreProfesor = s.Profesor != null ? $"{s.Profesor.Nombre} {s.Profesor.Apellido}" : null,
                    Estado = s.Estado.ToString(),
                    FechaSolicitud = s.FechaSolicitud,
                    FechaResolucion = s.FechaResolucion,
                    MensajeSocio = s.MensajeSocio,
                    MensajeProfesor = s.MensajeProfesor,
                    RutinaId = s.RutinaId,
                    Edad = s.Edad,
                    PesoKg = s.PesoKg,
                    AlturaCm = s.AlturaCm,
                    Nivel = s.Nivel,
                    SesionesPorSemana = s.SesionesPorSemana,
                    MinutosPorSesion = s.MinutosPorSesion,
                    Objetivo = s.Objetivo
                })
                .ToListAsync();
        }

        public async Task ActualizarAsync(SolicitudRutinaProfesor solicitud)
        {
            var existente = await _context.SolicitudesRutinaProfesor.FindAsync(solicitud.Id);
            if (existente != null)
            {
                _context.Entry(existente).CurrentValues.SetValues(solicitud);
            }
            await _context.SaveChangesAsync();
        }

        //obtener pofesor mas solicitado
        public async Task<Profesor?> ObtenerProfesorMasSolicitadoAsync()
        {
            return await _context.SolicitudesRutinaProfesor
                .Where(s => s.ProfesorId.HasValue)
                .GroupBy(s => s.Profesor)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();
        }

        //obtener profesor con mas pendientes
        public async Task<Profesor?> ObtenerProfesorConMasPendientesAsync()
        {
            return await _context.SolicitudesRutinaProfesor
                .Where(s => s.Estado == EstadoSolicitud.Pendiente && s.ProfesorId.HasValue)
                .GroupBy(s => s.Profesor)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();
        }

        //obtener Profesor más cumplidor (más solicitudes resueltas)
        public async Task<Profesor?> ObtenerProfesorMasCumplidorAsync()
        {
            return await _context.SolicitudesRutinaProfesor
                .Where(s => s.Estado == EstadoSolicitud.TomadaPorProfesor && s.ProfesorId.HasValue)
                .GroupBy(s => s.Profesor)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();
        }

        //obtener Profesor con mejor promedio de valoraciones y el promedio
        public async Task<(Profesor?, double?)?> ObtenerProfesorMejorPromedioValoracionesAsync()
        {
            var valoraciones = await _context.SolicitudesRutinaProfesor
                .Where(s => s.Rutina != null && s.Rutina.Valoraciones.Any() && s.ProfesorId.HasValue)
                .GroupBy(s => s.Profesor)
                .Select(g => new
                {
                    Profesor = g.Key,
                    Promedio = g.Average(x => x.Rutina!.Valoraciones.Average(v => v.Puntaje))
                })
                .OrderByDescending(x => x.Promedio)
                .FirstOrDefaultAsync();
            if (valoraciones != null)
            {
                return (valoraciones.Profesor, Math.Round(valoraciones.Promedio, 1));
            }
            return (null, null);
        }
    }
}
