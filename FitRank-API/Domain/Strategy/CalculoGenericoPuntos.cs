using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    public class CalculoGenerico
    {
        public ResultadoCalculo CalcularPuntos(
            Ejercicio ejercicio,
            int series,
            int repeticiones,
            double peso,
            Socio socio,
            MedidaCorporal ultimaMedida,
            double multiplicadorPeso,
            double multiplicadorReps)
        {
            // ✅ 1. Determinar peso máximo permitido (según progreso)
            double pesoMaximoPermitido = LogicaProgresion.ObtenerPesoMaximoPermitido(ejercicio, socio);
            bool pesoAjustado = false;
            string? advertencia = null;

            if (peso > pesoMaximoPermitido)
            {
                peso = pesoMaximoPermitido;
                pesoAjustado = true;
                advertencia = $"El peso ingresado supera el máximo permitido ({pesoMaximoPermitido} kg). Se ajustó automáticamente.";
            }

            // ✅ 2. Calcular factor del socio usando altura y su último peso
            double factorUsuario = ((ultimaMedida.PesoKg / 70.0) * 0.5) + ((socio.Altura / 175.0) * 0.5);

            // ✅ 3. Puntaje base
            double puntosBase = (series * repeticiones * multiplicadorReps) + (peso * multiplicadorPeso);

            // ✅ 4. Ajustar por factor usuario
            double puntosFinales = puntosBase * factorUsuario;

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
