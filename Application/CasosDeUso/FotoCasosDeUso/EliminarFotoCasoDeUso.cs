using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.FotoCasosDeUso
{
    public class EliminarFotoCasoDeUso
    {
        private readonly IFotoRepositorio _fotoRepositorio;

        public EliminarFotoCasoDeUso(IFotoRepositorio fotoRepositorio)
        {
            _fotoRepositorio = fotoRepositorio;
        }

        public async Task Ejecutar(long id)
        {
            await _fotoRepositorio.EliminarAsync(id);
        }
    }
}
