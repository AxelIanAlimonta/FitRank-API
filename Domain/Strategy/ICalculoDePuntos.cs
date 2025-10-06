using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    public interface ICalculoDePuntos
    {
        double CalcularPuntos(Ejercicio ejercicio,int series, int repeticiones , double peso, string TipoEntrenamiento, Usuario usuario);
    }
}
