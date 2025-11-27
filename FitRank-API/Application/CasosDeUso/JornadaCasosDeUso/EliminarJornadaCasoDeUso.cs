using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.JornadaCasosDeUso
{
    public class EliminarJornadaCasoDeUso
    {
        private readonly IJornadaRepositorio _jornadaRepositorio;
        public EliminarJornadaCasoDeUso(IJornadaRepositorio jornadaRepositorio)
        {
            _jornadaRepositorio = jornadaRepositorio;
        }
        public virtual async Task<bool> Ejecutar(long idJornada)
        {
            return await _jornadaRepositorio.EliminarJornadaAsync(idJornada);
        }
    }
}
