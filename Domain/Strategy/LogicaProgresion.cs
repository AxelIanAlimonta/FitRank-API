using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{

        public static class LogicaProgresion
        {
            public static double ObtenerPesoMaximoPermitido(Ejercicio ejercicio, Usuario usuario, double porcentajeIncremento = 0.2)
            {
                var ultimoRegistro = usuario.EjerciciosRealizados
                    .Where(e => e.Ejercicio.Id == ejercicio.Id)
                    .OrderByDescending(e => e.FechaRegistro)
                    .FirstOrDefault();

                if (ultimoRegistro == null)
                    return ejercicio.Peso;

                return ultimoRegistro.Peso * (1 + porcentajeIncremento);
            }
        }

    }

