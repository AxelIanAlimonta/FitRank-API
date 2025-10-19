using AutoMapper;
using FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios
{
    public class ObtenerTodasLasSesionesRealizadasDeEjerciciosCasoDeUso
    {
        private readonly ISesionRealizadaDeEjerciciosRepositorio _sesionRealizadaDeEjercicios;
        private readonly IMapper _mapper;

        public ObtenerTodasLasSesionesRealizadasDeEjerciciosCasoDeUso(ISesionRealizadaDeEjerciciosRepositorio SesionRealizadaDeEjercicios, IMapper mapper)
        {
            _sesionRealizadaDeEjercicios = SesionRealizadaDeEjercicios;
            _mapper = mapper;
        }
        public async Task<List<SesionRealizadaDeEjerciciosDTO>> Ejecutar()
        {
            var sesionesRealizadasDeEjercicios = await _sesionRealizadaDeEjercicios.ObtenerTodosAsync();
            return _mapper.Map<List<SesionRealizadaDeEjerciciosDTO>>(sesionesRealizadasDeEjercicios);
        }
    }
}
