using FitRank_API.Application.DTOs.EjercicioDTOs;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IEjercicioCatalogo
    {
        Task<IReadOnlyList<EjercicioRutinaGeneradaDTO>> BuscarAsync(CatalogoQuery query);
    }
    public sealed record CatalogoQuery(
        IReadOnlyCollection<string> Grupos,              // ej. ["Pecho","Espalda","Piernas","Hombro","Brazo","Core"]
        IReadOnlyCollection<string> EquiposPreferidos,  // ej. ["EQUIPO_MAQUINAS","EQUIPO_MANCUERNAS"]
        IReadOnlyCollection<string> EvitarUsuario,      // tags/palabras
        IReadOnlyCollection<string> Dolores             // ["Hombro","Rodilla","Lumbar"]
    );
}
