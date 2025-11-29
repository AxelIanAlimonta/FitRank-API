using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class ActualizarSocioCasoDeUso
{
    private readonly ISocioRepositorio _socioRepositorio;
    private readonly IMapper _mapper;
    public ActualizarSocioCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper)
    {
        _socioRepositorio = socioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<SocioDTO> Ejecutar(SocioDTO socioDTO)
    {
        var socio = _mapper.Map<Socio>(socioDTO);
        var socioActualizado = await _socioRepositorio.ActualizarAsync(socio);
        return _mapper.Map<SocioDTO>(socioActualizado);
    }


}
