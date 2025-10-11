using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.EjercicioRealizado
{
    public class EjercicioRealizadoConDivisionesDTO
    {
        public double PuntosObtenidos { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Diccionario: grupo muscular → división
        public Dictionary<GrupoMuscular, string> DivisionesPorGrupo { get; set; } = new();
    }
}
