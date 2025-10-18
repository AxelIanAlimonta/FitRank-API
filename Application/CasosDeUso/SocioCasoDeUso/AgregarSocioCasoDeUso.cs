using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class AgregarSocioCasoDeUso
{
    private readonly ISocioRepositorio _socioRepositorio;
    private readonly IMapper _mapper;

    public AgregarSocioCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper)
    {
        _socioRepositorio = socioRepositorio;
        _mapper = mapper;
    }

    public async Task<SocioDTO> Ejecutar(AgregarSocioDTO socio)
    {
        var socioEntidad = _mapper.Map<Socio>(socio);
        var socioCreado = await _socioRepositorio.AgregarAsync(socioEntidad);
        return _mapper.Map<SocioDTO>(socioCreado);

    }

}
