using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Strategy
{
    public class SeleccionDeCalculo
    {

        public static ICalculoDePuntos SeleccionarCalculo(Ejercicio ejercicio)
        {
            return ejercicio.GrupoMuscular switch
            {
                GrupoMuscular.Gluteos => new CalculoGluteo(),
                GrupoMuscular.Pecho => new CalculoPecho(),

                _ => throw new ArgumentException("Tipo de ejercicio no reconocido"),
            };
        }
    }

}
