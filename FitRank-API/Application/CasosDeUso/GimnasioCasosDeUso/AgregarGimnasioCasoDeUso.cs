using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class AgregarGimnasioCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    private readonly IMapper _mapper;
    public AgregarGimnasioCasoDeUso(IGimnasioRepositorio gimnasioRepositorio, IMapper mapper)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
        _mapper = mapper;
    }
    public async Task<ObtenerGimnasioDTO> Ejecutar(AgregarGimnasioDTO crearGimnasioDTO)
    {
        var gimnasioEntidad = _mapper.Map<Gimnasio>(crearGimnasioDTO);
        var gimnasioCreado = await _gimnasioRepositorio.AgregarGimnasio(gimnasioEntidad);
        return _mapper.Map<ObtenerGimnasioDTO>(gimnasioCreado);
    }

}
