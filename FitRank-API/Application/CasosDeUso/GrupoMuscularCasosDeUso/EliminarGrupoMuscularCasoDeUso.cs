using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;

public class EliminarGrupoMuscularCasoDeUso
{
    private readonly IGrupoMuscularRepositorio grupoMuscularRepositorio;
    public EliminarGrupoMuscularCasoDeUso(IGrupoMuscularRepositorio grupoMuscularRepositorio)
    {
        this.grupoMuscularRepositorio = grupoMuscularRepositorio;
    }
    public virtual async Task<bool> Ejecutar(long id)
    {
        return await grupoMuscularRepositorio.EliminarAsync(id);
    }
}
