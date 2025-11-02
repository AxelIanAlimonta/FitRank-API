using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class ActualizarGimnasioCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    private readonly IMapper _mapper;
    public ActualizarGimnasioCasoDeUso(IGimnasioRepositorio gimnasioRepositorio, IMapper mapper)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerGimnasioDTO?> Ejecutar(ActualizarGimnasioDTO gimnasioDto)
    {
        var gimnasioEntity = _mapper.Map<Gimnasio>(gimnasioDto);
        var gimnasioActualizado = await _gimnasioRepositorio.ActualizarGimnasio(gimnasioEntity);
        if (gimnasioActualizado == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerGimnasioDTO>(gimnasioActualizado);

    }

}
