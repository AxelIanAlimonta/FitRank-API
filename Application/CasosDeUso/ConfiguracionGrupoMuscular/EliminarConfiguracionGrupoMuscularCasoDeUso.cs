using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular
{
    public class EliminarConfiguracionGrupoMuscularCasoDeUso
    {
        private readonly IConfiguracionGrupoMuscularRepositorio _configuracionGrupoMuscularRepositorio;

        public EliminarConfiguracionGrupoMuscularCasoDeUso(IConfiguracionGrupoMuscularRepositorio configuracionGrupoMuscularRepositorio)
        {
            _configuracionGrupoMuscularRepositorio = configuracionGrupoMuscularRepositorio;
        }

        public async Task Ejecutar(long id)
        {
            await _configuracionGrupoMuscularRepositorio.EliminarAsync(id);
        }
    }
}
