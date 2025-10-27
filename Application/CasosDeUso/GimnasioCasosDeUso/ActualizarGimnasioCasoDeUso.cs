using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;

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

    public async Task<ObtenerGimnasioDTO?> Ejecutar(long id, ActualizarGimnasioDTO dto)
    {
        var gimnasioActualizado = await _gimnasioRepositorio.ActualizarGimnasio(id, dto);
        if (gimnasioActualizado is null) return null;

        return _mapper.Map<ObtenerGimnasioDTO>(gimnasioActualizado);
    }

}
