using System;
using System.Linq;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    public static class LogicaProgresion
    {
        /// <summary>
        /// Obtiene el peso máximo permitido para un ejercicio dado el historial del socio.
        /// - Busca la última actividad del socio para ese ejercicio.
        /// - Si no encuentra, intenta tomar el peso de la serie (serie recomendada).
        /// - Si no hay nada, usa un peso fallback.
        /// </summary>
        /// <param name="ejercicio">Ejercicio (no contiene peso)</param>
        /// <param name="socio">Socio (debe traer Entrenamientos, MedidasCorporales, etc.)</param>
        /// <param name="porcentajeIncremento">Incremento base permitido (ej. 0.2 = 20%)</param>
        /// <param name="pesoFallback">Peso por defecto si no hay historial/serie</param>
        /// <returns>kg redondeado a 2 decimales</returns>
        public static double ObtenerPesoMaximoPermitido(
            Ejercicio ejercicio,
            Socio socio,
            double porcentajeIncremento = 0.20,
            double pesoFallback = 10.0)
        {
            if (ejercicio == null) throw new ArgumentNullException(nameof(ejercicio));
            if (socio == null) throw new ArgumentNullException(nameof(socio));

            var ultimaActividad = socio.Entrenamientos?
                                        .SelectMany(t => t.Actividades ?? Enumerable.Empty<Actividad>())
                                        .Where(a => a.EjercicioAsignado != null && a.EjercicioAsignado.EjercicioId == ejercicio.Id)
                                        .OrderByDescending(a => a.Id) 
                                        .FirstOrDefault();

            double pesoAnterior = ultimaActividad?.Peso ?? 0.0;

            if (pesoAnterior == 0.0)
            {
                var algunaSerie = socio.Entrenamientos?
                                        .SelectMany(t => t.Actividades ?? Enumerable.Empty<Actividad>())
                                        .Select(a => a.Serie)
                                        .Where(s => s != null && s.EjercicioAsignado != null && s.EjercicioAsignado.EjercicioId == ejercicio.Id)
                                        .FirstOrDefault();

                if (algunaSerie != null && algunaSerie.Peso.HasValue)
                {
                    pesoAnterior = algunaSerie.Peso.Value;
                }
            }

            if (pesoAnterior == 0.0)
            {
                pesoAnterior = pesoFallback;
            }

            var ultimaMedida = socio.MedidasCorporales?
                                    .OrderByDescending(m => m.Fecha)
                                    .FirstOrDefault();

            double imc;
            if (ultimaMedida != null)
            {
                double alturaEnMetros = socio.Altura;
                if (alturaEnMetros <= 0) alturaEnMetros = 1.75;
                imc = ultimaMedida.PesoKg / Math.Pow(alturaEnMetros, 2);
            }
            else
            {
                imc = 22.0; 
            }

            double factorFisico = ejercicio.GrupoMuscular?
                .Configuraciones?
                .FirstOrDefault()?
                .FactorProgresion ?? 1.0;

            if (imc >= 25) factorFisico *= 1.05;
            else if (imc < 20) factorFisico *= 0.95;

            double diasInactividad = double.MaxValue;
            if (ultimaActividad != null)
            {
                diasInactividad = (DateTime.Today - (ultimaActividad.Entrenamiento?.Fecha.Date ?? DateTime.Today)).TotalDays;
            }
            else
            {
                diasInactividad = 999;
            }

            double factorInactividad = diasInactividad switch
            {
                > 60 => 0.8,
                > 30 => 0.9,
                _ => 1.0
            };

            double incrementoPermitido = porcentajeIncremento * factorFisico * factorInactividad;
            incrementoPermitido = Math.Min(incrementoPermitido, 0.4);

            double pesoMaximo = pesoAnterior * (1.0 + incrementoPermitido);
            return Math.Round(pesoMaximo, 2);
        }
    }
}
