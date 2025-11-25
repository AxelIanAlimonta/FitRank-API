using AutoMapper;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;

public class ObtenerDificultadPorIdCasoDeUso
{

    private readonly IDificultadRepositorio _dificultadRepositorio;
    private readonly IMapper _mapper;

    public ObtenerDificultadPorIdCasoDeUso(IDificultadRepositorio dificultadRepositorio, IMapper mapper)
    {
        _dificultadRepositorio = dificultadRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<DificultadDTO?> Ejecutar(int id)
    {
        var dificultadEntidad = await _dificultadRepositorio.ObtenerPorIdAsync(id);
        return dificultadEntidad == null ? null : _mapper.Map<DificultadDTO>(dificultadEntidad);
    }
}
