namespace FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs
{
    public class AgregarConfiguracionGrupoMuscularDTO
    {
        public int GrupoMuscularId { get; set; } // FK
        public double Multiplicadopeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }
    }
}
