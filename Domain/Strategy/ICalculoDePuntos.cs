using FitRank_API.Application.DTOs.EjercicioRealizado;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Strategy
{
    public interface ICalculoDePuntos
    {


        public ResultadoCalculo CalcularPuntos(Ejercicio ejercicio, int series, int repeticiones, double peso, string tipoEntrenamiento, Usuario usuario, double factorDificultad, double factorUsuario, double multiplicadorPeso, double multiplicadorReps);
    }
}
