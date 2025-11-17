using System;

namespace FitRank_API.Application.DTOs.FotoDTOs
{
    public class ObtenerFotoDTO
    {
        public long Id { get; set; }
        public long SocioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string UrlImagen { get; set; } = string.Empty;
    }
}

