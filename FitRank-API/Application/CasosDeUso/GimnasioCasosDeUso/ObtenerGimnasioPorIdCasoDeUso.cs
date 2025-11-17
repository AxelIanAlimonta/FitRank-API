using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class ObtenerGimnasioPorIdCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    private readonly IMapper _mapper;
    public ObtenerGimnasioPorIdCasoDeUso(IGimnasioRepositorio gimnasioRepositorio, IMapper mapper)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerGimnasioDTO?> Ejecutar(long id)
    {
        var gimnasio = await _gimnasioRepositorio.ObtenerGimnasioPorId(id);
        if (gimnasio == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerGimnasioDTO>(gimnasio);
    }
}
