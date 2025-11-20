using FitRank.API.Application.Rutinas.Abstractions;
using FitRank_API.Application.DTOs.RutinaDTOs;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IRoutineBuilder
{
    Task<RutinaGeneradaPorIADTO> BuildAsync(
        RutinaRequestDTO input,
        DecisionesRutinaDTO decisiones);
}
