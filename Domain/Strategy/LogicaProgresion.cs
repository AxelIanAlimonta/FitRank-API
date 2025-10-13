using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Strategy
{

        public static class LogicaProgresion
        {
        public static double ObtenerPesoMaximoPermitido(Ejercicio ejercicio, Usuario usuario, double porcentajeIncremento = 0.2)
        {
            var ultimoRegistro = usuario.ejerciciosRealizados
                .Where(e => e.Ejercicio.Id == ejercicio.Id)
                .OrderByDescending(e => e.FechaRegistro)
                .FirstOrDefault();

            if (ultimoRegistro == null)
                return ejercicio.Peso;

            double pesoAnterior = ultimoRegistro.Peso;
            double peso = usuario.pesoKg ?? 70.0;    // default 70 kg
            double altura = usuario.alturaCm ?? 175; // default 175 cm

            //IMC Aproximado
            double imc  = peso / Math.Pow(altura / 100.0, 2);

            double factorFisico = 1.0;

            // Los músculos grandes pueden progresar más rápido.
            factorFisico *= ejercicio.GrupoMuscular switch
            {
                Enums.GrupoMuscular.Piernas or GrupoMuscular.gluteo => 1.3,  // más margen
                GrupoMuscular.pecho or GrupoMuscular.Espalda => 1.1,
                GrupoMuscular.Hombros => 1.05,  
                GrupoMuscular.Brazos => 0.9, // más lento
                _ => 1.0
            };

            // Ajuste adicional por IMC: personas más pesadas o con más masa muscular pueden progresar más rápido
            if (imc >= 25)
                factorFisico *= 1.05; // más margen
            else if (imc < 20)
                factorFisico *= 0.95; // más conservador


            // Factor de inactividad ---
            double diasInactividad = (DateTime.Today - ultimoRegistro.FechaRegistro.Date).TotalDays;

            double factorInactividad = diasInactividad switch
            {
                > 60 => 0.8,  // estuvo más de 2 meses sin entrenar
                > 30 => 0.9,  // entre 1 y 2 meses sin entrenar
                _ => 1.0      // sigue activo
            };

            // Cálculo del incremento total (en porcentaje)
            double incrementoPermitido = porcentajeIncremento * factorFisico * factorInactividad;

            // Limitar el incremento máximo absoluto a +40%
            incrementoPermitido = Math.Min(incrementoPermitido, 0.4);

            // Calcular el nuevo peso máximo
            double pesoMaximo = pesoAnterior * (1 + incrementoPermitido);

            return Math.Round(pesoMaximo, 2);

        }
    }

    }

