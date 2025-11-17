using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.DTOs.SesionDTOs;
using System.Threading.Tasks;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IRutinaRepositorio
{
    Task<List<Rutina>> ObtenerTodasAsync();
    Task<Rutina?> ObtenerPorIdAsync(long id);
    Task<Rutina> AgregarAsync(Rutina rutina);
    Task<Rutina?> ActualizarAsync(Rutina rutina);
    Task<bool> EliminarAsync(long id);
    Task<Rutina> ObtenerPorSocioIdAsync(long socioId);
    Task<List<Rutina>> ObtenerRutinasPorSocioAsync(long socioId);
    Task<List<Rutina>> ObtenerFavoritasPorSocioAsync(long socioId);
    Task<bool> MarcarFavoritaAsync(long rutinaId, bool favorita);
    Task<bool> CambiarEstadoRutinaAsync(long rutinaId, bool activa);

    //RUTINA GENERADA POR IA
    Task<ResultadoConfirmarRutinaDTO> ValidarReferenciasAsync(ConfirmarRutinaDTO body);
    Task GuardarRutinaCompletaAsync(Rutina rutina, List<SesionIADTO> sesiones);
    Task<List<Rutina>> ObtenerTodasLasRutinasPorProfesorIdAsync(long profesorId);

   

}
