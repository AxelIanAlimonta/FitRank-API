using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class ObtenerRutinasFavoritasCasoDeUso
    {
        private readonly IRutinaRepositorio _repo;

        public ObtenerRutinasFavoritasCasoDeUso(IRutinaRepositorio repo)
        {
            _repo = repo;
        }

        public virtual async Task<List<ObtenerRutinaDTO>> Ejecutar(long socioId)
        {
            var rutinas = await _repo.ObtenerFavoritasPorSocioAsync(socioId);

            return rutinas.Select(r => new ObtenerRutinaDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                TipoCreacion = r.TipoCreacion,
                FechaCreacion = r.FechaCreacion,
                Descripcion = r.Descripcion,
                Activa = r.Activa,
                Favorita = r.Favorita,
                SocioId = r.SocioId,
                UsuarioId = r.UsuarioId
            }).ToList();
        }
    }
}
