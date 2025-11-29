using FitRank_API.Application.DTOs.EjercicioDTOs;

namespace FitRank_API.Domain.Interfaces
{
    public interface IEjercicioCatalogo
    {
        Task<IReadOnlyList<EjercicioRutinaGeneradaDTO>> BuscarAsync(CatalogoQuery query);
    }
    public sealed record CatalogoQuery(
        IReadOnlyCollection<string> Grupos,              
        IReadOnlyCollection<string> EquiposPreferidos,  
        IReadOnlyCollection<string> EvitarUsuario,      
        IReadOnlyCollection<string> Dolores             
    );
}
