using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    public  class CalculoGenerico : ICalculoDePuntos
    {
        public ResultadoCalculo CalcularPuntos(
        Ejercicio ejercicio,
        int series,
        int repeticiones,
        double peso,
       
        Usuario usuario,
        double factorDificultad,
        double factorUsuario,
        double multiplicadorPeso,
        double multiplicadorReps)
        {
            double pesoMaximoPermitido = LogicaProgresion.ObtenerPesoMaximoPermitido(ejercicio, usuario);
            bool pesoAjustado = false;
            string? advertencia = null;
            if (peso > pesoMaximoPermitido)
            {
                peso = pesoMaximoPermitido;
                pesoAjustado = true;
                advertencia = $"El peso ingresado supera el máximo permitido ({pesoMaximoPermitido} kg). Se ajustó automáticamente.";
            }

            double puntosBase = repeticiones * multiplicadorReps + peso * multiplicadorPeso;
            double puntosFinales = puntosBase * factorDificultad * factorUsuario;

            return new ResultadoCalculo
            {
                Puntos = Math.Round(puntosFinales, 2),
                PesoUsado = peso,
                PesoMaximoPermitido = pesoMaximoPermitido,
                PesoAjustado = pesoAjustado,
                MensajeAdvertencia = advertencia
            };
        }
    }
}
