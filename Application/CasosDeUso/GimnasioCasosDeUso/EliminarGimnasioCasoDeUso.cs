namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class EliminarGimnasioCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    public EliminarGimnasioCasoDeUso(IGimnasioRepositorio gimnasioRepositorio)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
    }
    public virtual async Task<bool> Ejecutar(long id)
    {
        return await _gimnasioRepositorio.EliminarGimnasio(id);
    }
}
