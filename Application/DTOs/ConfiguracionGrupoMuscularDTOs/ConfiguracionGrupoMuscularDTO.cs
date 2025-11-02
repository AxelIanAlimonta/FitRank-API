using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs
{
    public class ConfiguracionGrupoMuscularDTO
    {
        public long Id { get; set; }
        public int GrupoMuscularId { get; set; } // FK
        public double MultiplicadorPeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }
        public double FactorProgresion { get; set; }

    }
}
