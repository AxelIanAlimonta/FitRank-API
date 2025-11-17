namespace FitRank.API.Application.Rutinas.Abstractions
{
    public interface IRulesEvaluator
    {
        /// Ejecuta un workflow y devuelve las etiquetas (SuccessEvent) activadas.
        Task<IReadOnlyCollection<string>> EvaluateAsync(string workflowName, object input);

        /// Ejecuta todos los workflows conocidos y devuelve un mapa: workflow → etiquetas.
        Task<IDictionary<string, IReadOnlyCollection<string>>> EvaluateAllAsync(object input);
    }
}
