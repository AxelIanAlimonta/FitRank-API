using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface ISesionRealizadaDeEjerciciosRepositorio
{
    Task<List<SesionRealizadaDeEjercicios>> ObtenerTodosAsync();
    Task<SesionRealizadaDeEjercicios?> ObtenerPorIdAsync(long id);
    Task<SesionRealizadaDeEjercicios?> AgregarAsync(SesionRealizadaDeEjercicios grupoMuscular);
    Task<SesionRealizadaDeEjercicios?> ActualizarAsync(SesionRealizadaDeEjercicios grupoMuscular);
    Task EliminarAsync(long id);
}