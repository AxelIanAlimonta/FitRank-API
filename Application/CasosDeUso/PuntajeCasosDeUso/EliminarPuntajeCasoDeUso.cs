using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso
{
    public class EliminarPuntajeCasoDeUso
    {
        private readonly IPuntajeRepositorio _puntajeRepositorio;
        public EliminarPuntajeCasoDeUso(IPuntajeRepositorio puntajeRepositorio)
        {
            _puntajeRepositorio = puntajeRepositorio;
        }
        public async Task<bool> Ejecutar(long id)
        {
            return await _puntajeRepositorio.Eliminar(id);
        }
    }
}
