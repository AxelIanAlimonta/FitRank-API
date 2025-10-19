using AutoMapper;
using FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios
{
    public class ObtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso
    {
        private readonly ISesionRealizadaDeEjerciciosRepositorio _sesionRealizadaDeEjercicios;
        private readonly IMapper _mapper;

        public ObtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso(ISesionRealizadaDeEjerciciosRepositorio SesionRealizadaDeEjercicios, IMapper mapper)
        {
            _sesionRealizadaDeEjercicios = SesionRealizadaDeEjercicios;
            _mapper = mapper;
        }

        public async Task<SesionRealizadaDeEjerciciosDTO?> Ejecutar(long id)
        {
            var sesionRealizadaDeEjercicios = await _sesionRealizadaDeEjercicios.ObtenerPorIdAsync(id);
            return sesionRealizadaDeEjercicios == null ? null : _mapper.Map<SesionRealizadaDeEjerciciosDTO>(sesionRealizadaDeEjercicios);
        }
    }
}
