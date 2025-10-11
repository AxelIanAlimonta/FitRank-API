using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.Ranking
{
    public class MostrarRankingPorGrupoMuscular
    {
        public string userName { get; set; }
        public double TotalPuntos { get; set; }
        public string Nivel { get; set; }
        public GrupoMuscular GrupoMuscular { get; set; }
        public string Nombre { get; set; }

        public string DivisionPorGrupo { get; set; }

    }
}
