using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Application.DTOs.SerieDTOs;

namespace FitRank_API.Application.DTOs.SesionDTOs
{
    public record SesionIADTO(
        string Nombre,
        List<EjercicioAsignadoIADTO> Ejercicios,
        CardioIADTO? Cardio
    );
}
