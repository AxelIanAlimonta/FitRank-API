using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class CambiarEstadoRutinaCasoDeUso
    {
        private readonly IRutinaRepositorio _rutinaRepositorio;

        public CambiarEstadoRutinaCasoDeUso(IRutinaRepositorio rutinaRepositorio)
        {
            _rutinaRepositorio = rutinaRepositorio;
        }

        public virtual async Task<bool> Ejecutar(long rutinaId, bool activa)
        {
            return await _rutinaRepositorio.CambiarEstadoRutinaAsync(rutinaId, activa);
        }
    }
}
