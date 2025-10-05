namespace FitRank_API.Domain.Entities
{
  
        public class Ranking
        {
            public int Id { get; set; }

            
            public DateTime Fecha { get; set; }

      
            public int UsuarioId { get; set; }
            public Usuario Usuario { get; set; }

     
            public double PuntosTotales { get; set; }

            public int Posicion { get; set; }
        }

    }

