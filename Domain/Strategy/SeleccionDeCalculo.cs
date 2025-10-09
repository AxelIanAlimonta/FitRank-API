using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    public class SeleccionDeCalculo
    {

        public static ICalculoDePuntos SeleccionarCalculo(Ejercicio ejercicio)
        {
            return ejercicio.GrupoMuscular.ToLower() switch
            {
                "gluteo" => new CalculoGluteo(),
                "pecho" => new CalculoPecho(),

                _ => throw new ArgumentException("Tipo de ejercicio no reconocido"),
            };
        }
    }

}
