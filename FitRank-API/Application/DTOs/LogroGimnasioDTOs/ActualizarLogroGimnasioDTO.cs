namespace FitRank_API.Application.DTOs.LogroGimnasioDTOs
{
    public class ActualizarLogroGimnasioDTO
    {
        public long GimnasioId { get; set; }
        public long LogroId { get; set; }
        public bool EstaActivo { get; set; }
    }
}
