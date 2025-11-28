using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class ObtenerSociosCasoDeUso
{
    private readonly ISocioRepositorio _socioRepositorio;
    private readonly IMapper _mapper;

    public ObtenerSociosCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper)
    {
        _socioRepositorio = socioRepositorio;
        _mapper = mapper;
    }
    public virtual async Task<List<SocioDTO>> Ejecutar()
    {
        var socios = await _socioRepositorio.ObtenerTodosAsync();
        return _mapper.Map<List<SocioDTO>>(socios);

    }
}
