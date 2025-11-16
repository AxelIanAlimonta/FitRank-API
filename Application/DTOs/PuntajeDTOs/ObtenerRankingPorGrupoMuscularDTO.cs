namespace FitRank_API.Application.DTOs.PuntajeDTOs
{
    public class ObtenerRankingPorGrupoMuscularDTO
    {
        public long SocioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string GrupoMuscular { get; set; } = string.Empty;
        public double PuntajeTotal { get; set; }
    }
}
