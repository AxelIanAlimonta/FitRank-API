using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroCasosDeUso;

public class ObtenerLogrosCasoDeUso
{
    private readonly ILogroRepositorio _logroRepositorio;
    private readonly IMapper _mapper;
    public ObtenerLogrosCasoDeUso(ILogroRepositorio logroRepositorio, IMapper mapper)
    {
        _logroRepositorio = logroRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<List<ObtenerLogroDTO>> Ejecutar()
    {
        var logros = await _logroRepositorio.ObtenerTodosLosLogros();
        return _mapper.Map<List<ObtenerLogroDTO>>(logros);
    }
}
