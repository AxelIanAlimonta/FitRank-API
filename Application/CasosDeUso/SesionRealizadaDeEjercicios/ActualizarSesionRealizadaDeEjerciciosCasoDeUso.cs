using AutoMapper;
using FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios
{
    public class ActualizarSesionRealizadaDeEjerciciosCasoDeUso
    {
        private readonly ISesionRealizadaDeEjercicios _sesionRealizadaDeEjercicios;
        private readonly IMapper _mapper;

        public ActualizarSesionRealizadaDeEjerciciosCasoDeUso(ISesionRealizadaDeEjercicios SesionRealizadaDeEjercicios, IMapper mapper)
        {
            _sesionRealizadaDeEjercicios = SesionRealizadaDeEjercicios;
            _mapper = mapper;
        }

        public async Task<SesionRealizadaDeEjerciciosDTO?> Ejecutar(SesionRealizadaDeEjerciciosDTO sesionRealizadaDeEjerciciosDTO)
        {
            var sesionRealizadaDeEjerciciosEntidad = _mapper.Map<Domain.Entities.SesionRealizadaDeEjercicios>(sesionRealizadaDeEjerciciosDTO);
            var sesionRealizadaDeEjerciciosActualizado = await _sesionRealizadaDeEjercicios.ActualizarAsync(sesionRealizadaDeEjerciciosEntidad);
            return sesionRealizadaDeEjerciciosActualizado == null ? null : _mapper.Map<SesionRealizadaDeEjerciciosDTO>(sesionRealizadaDeEjerciciosActualizado);
        }
    }
}
