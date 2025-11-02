using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Actividad;

public class ObtenerActividadPorIdCasoDeUso
{
    private readonly IActividadRepositorio _repo;
    private readonly IMapper _mapper;

    public ObtenerActividadPorIdCasoDeUso(IActividadRepositorio repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerActividadDTO?> Ejecutar(long id)
    {
        var act = await _repo.ObtenerPorIdAsync(id);
        return act == null ? null : _mapper.Map<ObtenerActividadDTO>(act);
    }
}
