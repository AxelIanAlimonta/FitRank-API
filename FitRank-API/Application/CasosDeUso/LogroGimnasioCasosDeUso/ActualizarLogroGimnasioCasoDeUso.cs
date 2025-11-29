using AutoMapper;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso
{
    public class ActualizarLogroGimnasioCasoDeUso
    {
        private readonly ILogroGimnasioRepositorio _logroGimnasioRepositorio;
        private readonly ILogroRepositorio _logroRepositorio;
        private readonly IMapper _mapper;

        public ActualizarLogroGimnasioCasoDeUso(
            ILogroGimnasioRepositorio logroGimnasioRepositorio,
            ILogroRepositorio logroRepositorio,
            IMapper mapper)
        {
            _logroGimnasioRepositorio = logroGimnasioRepositorio;
            _logroRepositorio = logroRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<LogroGimnasioDTO?> Ejecutar(ActualizarLogroGimnasioDTO dto)
        {
            var logro = await _logroRepositorio.ObtenerLogroPorId(dto.LogroId);
            if (logro is null)
                return null;

            var existente = await _logroGimnasioRepositorio
                .ObtenerPorGimnasioYLogroAsync(dto.GimnasioId, dto.LogroId);

            LogroGimnasio entidad;

            if (existente is null)
            {
                entidad = _mapper.Map<LogroGimnasio>(dto);
                entidad.LogroId = dto.LogroId;
                entidad.GimnasioId = dto.GimnasioId;

                var creado = await _logroGimnasioRepositorio.CrearAsync(entidad);
                creado.Logro = logro;

                return _mapper.Map<LogroGimnasioDTO>(creado);
            }
            else
            {
                existente.EstaActivo = dto.EstaActivo;

                var actualizado = await _logroGimnasioRepositorio.ActualizarAsync(existente);
                actualizado!.Logro = logro;

                return _mapper.Map<LogroGimnasioDTO>(actualizado);
            }
        }
    }
}
