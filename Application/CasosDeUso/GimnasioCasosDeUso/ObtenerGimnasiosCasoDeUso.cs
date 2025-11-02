using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class ObtenerGimnasiosCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    private readonly IMapper _mapper;
    public ObtenerGimnasiosCasoDeUso(IGimnasioRepositorio gimnasioRepositorio, IMapper mapper)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<List<ObtenerGimnasioDTO>> Ejecutar()
    {
        var gimnasios = await _gimnasioRepositorio.ObtenerTodosLosGimnasios();
        return _mapper.Map<List<ObtenerGimnasioDTO>>(gimnasios);
    }
}
