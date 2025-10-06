using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    
    public class CalculoGluteo : ICalculoDePuntos
    {
        public double CalcularPuntos(Ejercicio ejercicio, int series, int repeticiones, double peso, string TipoEntrenamiento, Usuario usuario)
        {
            double pesoMaximoPermitido = LogicaProgresion.ObtenerPesoMaximoPermitido(ejercicio, usuario);

            if (peso > pesoMaximoPermitido)
                peso = pesoMaximoPermitido;

            double puntos =  repeticiones * 0.1 + peso * 0.1;
            return puntos;
        }
    }
    }

