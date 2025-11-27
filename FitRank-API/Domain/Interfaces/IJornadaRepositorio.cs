using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface IJornadaRepositorio
    {
        Task<List<Jornada>> ObtenerTodasLasJornadasAsync();
        Task<Jornada?> ObtenerJornadaPorIdAsync(long id);
        Task<Jornada> AgregarJornadaAsync(Jornada nuevaJornada);
        Task<Jornada?> ActualizarJornadaAsync(Jornada jornadaActualizada);
        Task<bool> EliminarJornadaAsync(long id);
    }
}
