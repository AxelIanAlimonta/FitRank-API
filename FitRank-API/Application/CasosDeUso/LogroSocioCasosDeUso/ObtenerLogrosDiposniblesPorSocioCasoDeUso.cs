using AutoMapper;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso
{
    public class ObtenerLogrosDisponiblesPorSocioCasoDeUso
    {
        private readonly ILogroGimnasioRepositorio _logroGimnasioRepositorio;
        private readonly ILogroSocioRepositorio _logroSocioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerLogrosDisponiblesPorSocioCasoDeUso(
            ILogroGimnasioRepositorio logroGimnasioRepositorio,
            ILogroSocioRepositorio logroSocioRepositorio,
            IMapper mapper)
        {
            _logroGimnasioRepositorio = logroGimnasioRepositorio;
            _logroSocioRepositorio = logroSocioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<LogroDisponibleDTO>> Ejecutar(int socioId, int gimnasioId)
        {
            // 1) Config de logros del gimnasio (incluye Logro)
            var configGimnasio = await _logroGimnasioRepositorio
                .ObtenerPorGimnasioAsync(gimnasioId);

            var logrosHabilitados = configGimnasio
                .Where(lg => lg.EstaActivo && lg.Logro != null)
                .ToList();

            // 2) Logros que el socio YA tiene en ese gimnasio
            var logrosSocio = await _logroSocioRepositorio
                .ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId);

            var idsLogrosSocio = logrosSocio
                .Select(ls => ls.LogroId)
                .ToHashSet();

            // 3) Filtrar: habilitados que el socio todavía no ganó
            var disponibles = logrosHabilitados
                .Where(lg => !idsLogrosSocio.Contains(lg.LogroId))
                .ToList();

            var resultado = disponibles.Select(lg => new LogroDisponibleDTO
            {
                LogroId = lg.LogroId,
                Nombre = lg.Logro.Nombre,
                NombreClave = lg.Logro.NombreClave,
                Descripcion = lg.Logro.Descripcion,
                Imagen= lg.Logro.Imagen,
            });

            return resultado;
        }
    }
}
