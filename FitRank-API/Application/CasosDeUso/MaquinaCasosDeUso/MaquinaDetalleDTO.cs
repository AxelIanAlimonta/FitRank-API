using FitRank_API.Application.DTOs.MaquinaDTOs;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class MaquinaDetalleDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string? UrlImagen { get; set; }
        public string Qr { get; set; }

        public List<EjercicioDeMaquinaDTO> Ejercicios { get; set; } = new();
    }
}
