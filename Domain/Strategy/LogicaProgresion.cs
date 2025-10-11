using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{

        public static class LogicaProgresion
        {
            public static double ObtenerPesoMaximoPermitido(Ejercicio ejercicio, Usuario usuario, double porcentajeIncremento = 0.2)
            {
                var ultimoRegistro = usuario.ejerciciosRealizados
                    .Where(er => er.EjercicioId == ejercicio.Id)
                    .OrderByDescending(er => er.FechaRegistro)
                    .FirstOrDefault();

            if (ultimoRegistro == null)
                    return ejercicio.Peso;

                return ultimoRegistro.Peso * (1 + porcentajeIncremento);
            }
        }

    }

