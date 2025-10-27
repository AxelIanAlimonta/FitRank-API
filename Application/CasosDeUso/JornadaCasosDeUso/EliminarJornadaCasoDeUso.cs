using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.JornadaCasosDeUso
{
    public class EliminarJornadaCasoDeUso
    {
        private readonly IJornadaRepositorio _jornadaRepositorio;
        public EliminarJornadaCasoDeUso(IJornadaRepositorio jornadaRepositorio)
        {
            _jornadaRepositorio = jornadaRepositorio;
        }
        public async Task<bool> Ejecutar(long idJornada)
        {
            return await _jornadaRepositorio.EliminarJornadaAsync(idJornada);
        }
    }
}
