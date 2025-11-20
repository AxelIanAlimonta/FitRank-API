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

            // 1) Buscar última actividad realizada por el socio para ese ejercicio
            var ultimaActividad = socio.Entrenamientos?
                                        .SelectMany(t => t.Actividades ?? Enumerable.Empty<Actividad>())
                                        .Where(a => a.EjercicioAsignado != null && a.EjercicioAsignado.EjercicioId == ejercicio.Id)
                                        .OrderByDescending(a => a.Id) // o por fecha si la actividad tuviera fecha explícita
                                        .FirstOrDefault();

            double pesoAnterior = ultimaActividad?.Peso ?? 0.0;

            // 2) Si no hay actividad, intentar tomar peso de una serie recomendada (si existe)
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

            // 3) Si todavía es 0, fallback
            if (pesoAnterior == 0.0)
            {
                // Podés ajustar este fallback (10kg por ejemplo)
                pesoAnterior = pesoFallback;
            }

            // 4) Obtener IMC a partir de la última medida corporal disponible
            var ultimaMedida = socio.MedidasCorporales?
                                    .OrderByDescending(m => m.Fecha)
                                    .FirstOrDefault();

            double imc;
            if (ultimaMedida != null)
            {
                // IMPORTANT: asegúrate que socio.Altura está en METROS. 
                // Si la tenés en cms usá: socio.Altura / 100.0
                double alturaEnMetros = socio.Altura;
                if (alturaEnMetros <= 0) alturaEnMetros = 1.75; // fallback razonable
                imc = ultimaMedida.PesoKg / Math.Pow(alturaEnMetros, 2);
            }
            else
            {
                imc = 22.0; // valor neutro si no hay medida
            }

            // 5) Factor por grupo muscular (usando el nombre del grupo)
            // ✅ Obtener el factor de progresión dinámico desde la configuración del grupo muscular
            double factorFisico = ejercicio.GrupoMuscular?
                .Configuraciones?
                .FirstOrDefault()?
                .FactorProgresion ?? 1.0;

            // 6) Ajuste por IMC
            if (imc >= 25) factorFisico *= 1.05;
            else if (imc < 20) factorFisico *= 0.95;

            // 7) Factor de inactividad (tomamos fecha del entrenamiento de la última actividad)
            double diasInactividad = double.MaxValue;
            if (ultimaActividad != null)
            {
                // asumimos que la actividad tiene referencia al entrenamiento con fecha
                diasInactividad = (DateTime.Today - (ultimaActividad.Entrenamiento?.Fecha.Date ?? DateTime.Today)).TotalDays;
            }
            else
            {
                // si no hay actividad reciente, consideramos larga inactividad
                diasInactividad = 999;
            }

            double factorInactividad = diasInactividad switch
            {
                > 60 => 0.8,
                > 30 => 0.9,
                _ => 1.0
            };

            // 8) cálculo final del incremento permitido
            double incrementoPermitido = porcentajeIncremento * factorFisico * factorInactividad;
            incrementoPermitido = Math.Min(incrementoPermitido, 0.4); // tope 40%

            double pesoMaximo = pesoAnterior * (1.0 + incrementoPermitido);
            return Math.Round(pesoMaximo, 2);
        }
    }
}
