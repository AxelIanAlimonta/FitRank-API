namespace FitRank_API.Application.DTOs.LogroDTOs
{
    public class LogroOtorgadoDTO
    {
        public long LogroId { get; set; }
        public long SocioId { get; set; }
        public long GimnasioId { get; set; }
        public string Nombre { get; set; }
        public string NombreClave { get; set; }
        public DateTime FechaOtorgado { get; set; }
        public bool Otorgado { get; set; }
        public string? Motivo { get; set; }
    }

}
