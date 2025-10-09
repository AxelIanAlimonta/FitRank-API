namespace FitRank_API.Application.DTOs.EjercicioRealizado
{
    public class EjercicioRealizadoDTOSalida
    {

     
            public int Id { get; set; }
            public int UsuarioId { get; set; }
            public int EjercicioId { get; set; }

            // Asegúrate de declarar estas propiedades
            public string NombreEjercicio { get; set; }
            public string GrupoMuscular { get; set; }

            public int Series { get; set; }
            public int Repeticiones { get; set; }
            public double Peso { get; set; }

            public double PuntosObtenidos { get; set; }
            public DateTime FechaRegistro { get; set; }
            public string Observacion { get; set; }
        }


    }

