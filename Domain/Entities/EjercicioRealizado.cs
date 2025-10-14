using System.ComponentModel.DataAnnotations.Schema;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Entities
{


    public class EjercicioRealizado
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int? AsistenciaId { get; set; }
        public Asistencia Asistencia { get; set; }

        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }

        public int BloqueId { get; set; } //DUDAS SI DEJARLO
        public Bloque bloque { get; set; } //DUDAS SI DEJARLO

        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public double Peso { get; set; }
        public Dificultad Dificultad { get; set; }

        public double PuntosObtenidos { get; set; }
        public string ObservacionDelUsuario { get; set; }

       
       
        public DateTime FechaRegistro { get; set; } 
    }
}
