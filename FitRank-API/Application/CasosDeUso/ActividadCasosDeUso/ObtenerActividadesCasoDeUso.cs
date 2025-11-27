using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.UseCases.Actividad;

public class ObtenerActividadesCasoDeUso
{
    private readonly IActividadRepositorio _repo;
    private readonly IMapper _mapper;

    public ObtenerActividadesCasoDeUso(IActividadRepositorio repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<ObtenerActividadDTO>> Ejecutar()
    {
        var actividades = await _repo.ObtenerTodasAsync();
        return _mapper.Map<IEnumerable<ObtenerActividadDTO>>(actividades);
    }
}
