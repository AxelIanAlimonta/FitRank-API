using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class EliminarProfesorCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        public EliminarProfesorCasoDeUso(IProfesorRepositorio profesorRepositorio)
        {
            _profesorRepositorio = profesorRepositorio;
        }
        public virtual async Task<bool> Ejecutar(long id)
        {
            return await _profesorRepositorio.EliminarAsync(id);
        }
    }
}
