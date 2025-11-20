using FitRank_API.Application.DTOs.SesionDTOs;

namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public record RutinaGeneradaPorIADTO(
        string Nombre,
        string Objetivo,
        string Division,
        int Sesiones,
        int MinutosPorSesion,
        List<SesionIADTO> SesionesPlan,
        object InputSnapshot,
        object RulesExplain
    );
}
