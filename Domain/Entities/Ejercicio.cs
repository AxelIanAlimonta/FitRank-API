namespace FitRank_API.Domain.Entities
{
    public class Ejercicio
    {
        public int Id { get; set; }
        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }

        public int MaquinaId { get; set; }
        public Maquina Maquina { get; set; }

        public string Nombre { get; set; }
        public string GrupoMuscular { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public double Peso { get; set; }
        public int DescansoSegundos { get; set; }
        public bool EsSerieCompuesta { get; set; }
        public bool EsOpcional { get; set; }
        public DayOfWeek DiaAsignado { get; set; }
        public string Observaciones { get; set; }
        public string VideoUrl { get; set; }


    }
}
