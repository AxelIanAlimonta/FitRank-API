using AutoMapper;
using FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios
{
    public class AgregarSesionRealizadaDeEjerciciosCasoDeUso
    {
        private readonly ISesionRealizadaDeEjercicios _sesionRealizadaDeEjercicios;
        private readonly IMapper _mapper;

        public AgregarSesionRealizadaDeEjerciciosCasoDeUso(ISesionRealizadaDeEjercicios SesionRealizadaDeEjercicios, IMapper mapper)
        {
            _sesionRealizadaDeEjercicios = SesionRealizadaDeEjercicios;
            _mapper = mapper;
        }
        public async Task<SesionRealizadaDeEjerciciosDTO> Ejecutar(AgregarSesionRealizadaDeEjerciciosDTO agregarSesionRealizadaDeEjerciciosDTO)
        {
            var sesionRealizadaDeEjerciciosEntidad = _mapper.Map<Domain.Entities.SesionRealizadaDeEjercicios>(agregarSesionRealizadaDeEjerciciosDTO);
            var sesionRealizadaDeEjerciciosCreado = await _sesionRealizadaDeEjercicios.AgregarAsync(sesionRealizadaDeEjerciciosEntidad);
            return _mapper.Map<SesionRealizadaDeEjerciciosDTO>(sesionRealizadaDeEjerciciosCreado);
        }
    }
}
