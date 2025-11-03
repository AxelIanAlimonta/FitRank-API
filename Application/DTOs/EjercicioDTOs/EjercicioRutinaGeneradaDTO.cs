namespace FitRank_API.Application.DTOs.EjercicioDTOs
{
    public record EjercicioRutinaGeneradaDTO(
    long Id,
    string Nombre,
    string Tipo,                 // Podés usar GrupoMuscular.Nombre o tu enum TipoEjercicio
    string EquipoNecesario,      // "Maquina" | "Mancuernas" | "Barra" | "Polea" | "Libre"
    List<string> Tags,
    List<string> ContraIndicaciones);
}
