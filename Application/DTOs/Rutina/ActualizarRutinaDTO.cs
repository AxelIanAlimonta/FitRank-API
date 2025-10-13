namespace FitRank_API.Application.DTOs.Rutina
{
    public class ActualizarRutinaDTO
    {
        public string Nombre {  get; set; }
        public int FrecuenciaSemanal {  get; set; }
        public List<ActualizarBloqueRutinaDTO> Bloques { get; set; } = new();
    }

    public class ActualizarBloqueRutinaDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<ActualizarBloqueDiaDTO> Dias { get; set; } = new();
        public List<ActualizarEjercicioBloqueDTO> Ejercicios { get; set; } = new();
    }

    public class ActualizarBloqueDiaDTO
    {
        public int IdDia { get; set; }
    }

    public class ActualizarEjercicioBloqueDTO
    {
        public int IdEjercicio { get; set; }
        public int Orden { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int Rir { get; set; }
        public decimal? Peso { get; set; }
    }

    public class ActualizarDia {
        public int IdDia { get; set; }
        public string Nombre { get; set; }
    }
}
