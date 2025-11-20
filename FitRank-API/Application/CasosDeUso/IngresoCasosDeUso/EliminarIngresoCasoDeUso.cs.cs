using FitRank_API.Infrastructure.Interfaces;


namespace FitRank_API.Application.CasosDeUso.Ingreso
{
    public class EliminarIngresoCasoDeUso
    {
        private readonly IIngresoRepositorio _repo;

        public EliminarIngresoCasoDeUso(IIngresoRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<bool> Ejecutar(long id)
        {
            var ingreso = await _repo.ObtenerPorIdAsync(id);
            if (ingreso == null) return false;

            await _repo.EliminarAsync(ingreso);
            return true;
        }
    }
}
