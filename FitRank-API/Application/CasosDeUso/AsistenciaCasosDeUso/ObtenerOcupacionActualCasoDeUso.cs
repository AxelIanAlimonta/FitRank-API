using System.Threading.Tasks;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class ObtenerOcupacionActualCasoDeUso
    {
        private readonly IAsistenciaRepositorio _repo;

        public ObtenerOcupacionActualCasoDeUso(IAsistenciaRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<int> Ejecutar(long gimnasioId)
        {
            return await _repo.ObtenerOcupacionActualAsync(gimnasioId);
        }
    }
}
