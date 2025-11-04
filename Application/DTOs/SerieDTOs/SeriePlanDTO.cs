namespace FitRank_API.Application.DTOs.SerieDTOs
{
    public class SeriePlanDTO
    {
        public int Nro { get; set; }                // número de serie
        public int Reps { get; set; }               // cantidad de repeticiones
        public double? PesoObjetivo { get; set; }
    }
}
