namespace FitRank_API.Application.DTOs.Logro
{
    public class LogroUsuarioDto
    {
        public int LogroId { get; set; }
        public string Nombre { get; set; } = null!;
        public int PuntosOtorgados { get; set; }
        public DateTime FechaOtorgado { get; set; }

        public LogroUsuarioDto() { }

        public LogroUsuarioDto(int logroId, string nombre, int puntosOtorgados, DateTime fechaOtorgado)
        {
            LogroId = logroId;
            Nombre = nombre;
            PuntosOtorgados = puntosOtorgados;
            FechaOtorgado = fechaOtorgado;
        }
    }
}
