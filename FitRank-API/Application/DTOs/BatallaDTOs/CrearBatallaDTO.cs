using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.BatallaDTOs
{
    public class CrearBatallaDTO
    {
        public int SocioAId { get; set; }
        public int SocioBId { get; set; }
        public BatallaTipo Tipo { get; set; }
        public int DiasDuracion { get; set; } 
    }
}
