using AutoMapper;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionCasosDeUso;

public class AgregarSesionCasoDeUso
{
    private readonly ISesionRepositorio _sesionRepositorio;
    private readonly IMapper _mapper;

    public AgregarSesionCasoDeUso(ISesionRepositorio sesionRepositorio, IMapper mapper)
    {
        _sesionRepositorio = sesionRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerSesionDTO> Ejecutar(AgregarSesionDTO nuevaSesion)
    {
        var sesionEntidad = _mapper.Map<Domain.Entities.Sesion>(nuevaSesion);
        var sesionAgregada = await _sesionRepositorio.AgregarAsync(sesionEntidad);
        return _mapper.Map<ObtenerSesionDTO>(sesionAgregada);
    }

}
