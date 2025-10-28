namespace FitRank_API.Application.DTOs
{
    public class ActualizarGrupoMuscularDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Imagen { get; set; }
    }
}
