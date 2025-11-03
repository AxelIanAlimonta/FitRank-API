namespace FitRank.API.Application.Rutinas.Abstractions
{
    public interface IRoutineRulesRunner
    {
        Task<DecisionesRutinaDTO> RunAsync(object input);
    }
}
