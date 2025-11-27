using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class ObtenerSocioPorIdCasoDeUso
{

    private readonly ISocioRepositorio _socioRepositorio;
    private readonly IMapper _mapper;

    public ObtenerSocioPorIdCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper)
    {
        _socioRepositorio = socioRepositorio;
        _mapper = mapper;
    }


    public virtual async Task<SocioDTO?> Ejecutar(long id)
    {
        var socio = await _socioRepositorio.ObtenerPorIdAsync(id);
        return socio == null ? null : _mapper.Map<SocioDTO>(socio);
    }


}
