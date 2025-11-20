using FitRank_API.Application.DTOs.SerieDTOs;

namespace FitRank_API.Application.DTOs.EjercicioAsignadoDTOs
{
    public record EjercicioAsignadoIADTO(
        long EjercicioId,
        string Nombre,
        string Tipo,               // p.ej. "Pecho", "Espalda", "Piernas", "Hombro", "Brazo", "Core"
        string Equipo,             // "Maquina" / "Mancuernas" / "Barra" / "Polea" / "Libre"
        List<SerieAsignadaIADTO> Series,
        List<string> Tags,
        List<string> ContraIndicaciones
    );
}
