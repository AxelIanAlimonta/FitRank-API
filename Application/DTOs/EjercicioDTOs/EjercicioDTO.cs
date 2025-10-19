using FitRank_API.Application.DTOs.GrupoMuscularDTOs;

namespace FitRank_API.Application.DTOs.EjercicioDTOs;

public class EjercicioDTO
{
    public long Id { set; get; }
    public string Nombre { set; get; } = string.Empty;
    public string UrlVideo { set; get; } = string.Empty;
    public long GrupoMuscularId { set; get; }
    GrupoMuscularDTO GrupoMuscular { set; get; } = new GrupoMuscularDTO();
}
