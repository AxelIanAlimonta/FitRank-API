using FitRank.API.Application.Rutinas.Abstractions;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

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
            // Ejecuta las reglas del motor
            var decisiones = await _rulesRunner.RunAsync(input);

            // Si las reglas indican derivación, devolvemos un resultado especial
            if (decisiones.DerivarProfesional)
            {
                return new ResultadoGenerarRutinaDTO
                {
                    RequiereDerivacion = true,
                    Mensaje = "Se requiere derivación/validación profesional", //mensaje especifico del porque no se pudo crear la rutina
                    Decisiones = decisiones,
                    Rutina = null
                };
            }

            // Si no hay derivación, construimos la rutina
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
