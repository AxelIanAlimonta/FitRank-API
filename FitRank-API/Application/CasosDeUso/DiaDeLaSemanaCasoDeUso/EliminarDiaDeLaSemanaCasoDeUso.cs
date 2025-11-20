using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso
{
    public class EliminarDiaDeLaSemanaCasoDeUso
    {
        private readonly IDiaDeLaSemanaRepositorio _diaDeLaSemanaRepositorio;
        public EliminarDiaDeLaSemanaCasoDeUso(IDiaDeLaSemanaRepositorio diaDeLaSemanaRepositorio)
        {
            _diaDeLaSemanaRepositorio = diaDeLaSemanaRepositorio;
        }
        public virtual async Task<bool> Ejecutar(long id)
        {
            return await _diaDeLaSemanaRepositorio.EliminarDiaDeLaSemanaAsync(id);
        }
    }
}
