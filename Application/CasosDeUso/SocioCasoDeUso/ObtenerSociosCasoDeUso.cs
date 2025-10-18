using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

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
    public async Task<IEnumerable<SocioDTO>> Ejecutar()
    {
        var socios = await _socioRepositorio.ObtenerTodosAsync();
        return _mapper.Map<IEnumerable<SocioDTO>>(socios);

    }
}
