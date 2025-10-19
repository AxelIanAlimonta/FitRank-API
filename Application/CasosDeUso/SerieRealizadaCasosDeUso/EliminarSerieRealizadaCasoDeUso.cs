using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso
{
    public class EliminarSerieRealizadaCasoDeUso
    {
        private readonly ISerieRealizadaRepositorio _serieRealizadaRepositorio;
        public EliminarSerieRealizadaCasoDeUso(ISerieRealizadaRepositorio serieRealizadaRepositorio)
        {
            _serieRealizadaRepositorio = serieRealizadaRepositorio;
        }
        public async Task<bool> Ejecutar(long id)
        {
            return await _serieRealizadaRepositorio.EliminarAsync(id);
        }
    }
}
