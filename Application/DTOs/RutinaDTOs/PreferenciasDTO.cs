namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public sealed class PreferenciasDTO
    {
        public bool IncluirCardio { get; set; }
        public bool PrefiereMaquinas { get; set; }
        public bool PrefiereMancuernas { get; set; }

        public List<string> EjerciciosExcluidos { get; set; } = new();
    }
}
