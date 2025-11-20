using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs
{
    public class ActualizarGrupoMuscularDTO
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "El nombre del grupo muscular es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;
        public string? Imagen { get; set; }
    }
}
