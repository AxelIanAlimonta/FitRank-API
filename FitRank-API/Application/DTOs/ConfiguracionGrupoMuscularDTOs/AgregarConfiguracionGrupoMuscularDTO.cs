namespace FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs
{
    public class AgregarConfiguracionGrupoMuscularDTO
    {
        public int GrupoMuscularId { get; set; } // FK
        public double MultiplicadorPeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }
        public double FactorProgresion { get; set; }
    }
}
