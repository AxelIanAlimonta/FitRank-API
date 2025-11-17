using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;

public class AgregarRutinaCasoDeUso
{
    private readonly IRutinaRepositorio _rutinaRepositorio;
    private readonly IMapper _mapper;

    public AgregarRutinaCasoDeUso(IRutinaRepositorio rutinaRepositorio, IMapper mapper)
    {
        _rutinaRepositorio = rutinaRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerRutinaDTO> Ejecutar(AgregarRutinaDTO nuevaRutina)
    {
        var rutinaEntidad = _mapper.Map<Domain.Entities.Rutina>(nuevaRutina);
        var rutinaAgregada = await _rutinaRepositorio.AgregarAsync(rutinaEntidad);
        return _mapper.Map<ObtenerRutinaDTO>(rutinaAgregada);
    }

}
