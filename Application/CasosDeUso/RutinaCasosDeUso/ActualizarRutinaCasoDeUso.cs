using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;

public class ActualizarRutinaCasoDeUso
{
    private readonly IRutinaRepositorio _rutinaRepositorio;
    private readonly IMapper _mapper;

    public ActualizarRutinaCasoDeUso(IRutinaRepositorio rutinaRepositorio, IMapper mapper)
    {
        _rutinaRepositorio = rutinaRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerRutinaDTO?> Ejecutar(ActualizarRutinaDTO rutinaActualizada)
    {
        var rutinaEntidad = _mapper.Map<Rutina>(rutinaActualizada);
        var rutinaActualizadaEntidad = await _rutinaRepositorio.Actualizar(rutinaEntidad);
        if (rutinaActualizadaEntidad == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerRutinaDTO>(rutinaActualizadaEntidad);
    }


}
