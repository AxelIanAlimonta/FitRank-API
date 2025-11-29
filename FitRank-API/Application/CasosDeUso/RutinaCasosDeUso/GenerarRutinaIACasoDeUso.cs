using FitRank.API.Application.Rutinas.Abstractions;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class GenerarRutinaIACasoDeUso
    {
        private readonly IRoutineRulesRunner _rulesRunner;
        private readonly IRoutineBuilder _builder;


        public GenerarRutinaIACasoDeUso(
            IRoutineRulesRunner rulesRunner,
            IRoutineBuilder builder)
        {
            _rulesRunner = rulesRunner;
            _builder = builder;
        }

        public virtual async Task<ResultadoGenerarRutinaDTO> EjecutarAsync(RutinaRequestDTO input)
        {
            var decisiones = await _rulesRunner.RunAsync(input);

            if (decisiones.DerivarProfesional)
            {
                return new ResultadoGenerarRutinaDTO
                {
                    RequiereDerivacion = true,
                    Mensaje = "Se requiere derivación/validación profesional", 
                    Decisiones = decisiones,
                    Rutina = null
                };
            }

            var rutina = await _builder.BuildAsync(input, decisiones);

            return new ResultadoGenerarRutinaDTO
            {
                RequiereDerivacion = false,
                Mensaje = "Rutina generada correctamente",
                Decisiones = decisiones,
                Rutina = rutina
            };
        }
    }
}
