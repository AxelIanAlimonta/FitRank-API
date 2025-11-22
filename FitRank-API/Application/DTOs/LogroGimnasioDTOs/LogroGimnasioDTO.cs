namespace FitRank_API.Application.DTOs.LogroGimnasioDTOs
{
    public class LogroGimnasioDTO
    {
        public long GimnasioId { get; set; }
        public long LogroId { get; set; }
        public bool EstaHabilitado { get; set; }


        // datos del logro global para mostrar en el panel
        public string Nombre { get; set; }
        public string NombreClave { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
    }
}
