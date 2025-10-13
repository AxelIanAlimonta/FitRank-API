namespace FitRank_API.Application.DTOs.Logro
{
    public class LogroDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int Puntos { get; set; }
        public bool Activo { get; set; }

        public LogroDto() { }

        public LogroDto(int id, string nombre, string descripcion, int puntos, bool activo)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Puntos = puntos;
            Activo = activo;
        }
    }
}
