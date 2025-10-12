
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Strategy
{
    public class ObtenerPuntos
    {
        public static double ObtenerPuntosTotales(Usuario usuario)
        {
            // Suma total de puntos
            return usuario.ejerciciosRealizados?.Sum(e => e.PuntosObtenidos) ?? 0;
        }

        public static double ObtenerPuntosPorGrupo(Usuario usuario, GrupoMuscular grupo)
        {
            return usuario.ejerciciosRealizados?
                .Where(e => e.Ejercicio.GrupoMuscular == grupo)
                .Sum(e => e.PuntosObtenidos) ?? 0;
        }
    }
    
}
