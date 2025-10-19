using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios
{
    public class EliminarSesionRealizadaDeEjerciciosCasoDeUso
    {
        private readonly ISesionRealizadaDeEjercicios _sesionRealizadaDeEjercicios;

        public EliminarSesionRealizadaDeEjerciciosCasoDeUso(ISesionRealizadaDeEjercicios SesionRealizadaDeEjercicios)
        {
            _sesionRealizadaDeEjercicios = SesionRealizadaDeEjercicios;
        }

        public async Task Ejecutar(long id)
        {
            await _sesionRealizadaDeEjercicios.EliminarAsync(id);
        }
    }
}
