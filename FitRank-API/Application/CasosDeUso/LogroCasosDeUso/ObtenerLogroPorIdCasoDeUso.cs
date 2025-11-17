using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroCasosDeUso;

public class ObtenerLogroPorIdCasoDeUso
{
    private readonly ILogroRepositorio _logroRepositorio;
    IMapper _mapper;
    public ObtenerLogroPorIdCasoDeUso(ILogroRepositorio logroRepositorio, IMapper mapper)
    {
        _logroRepositorio = logroRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerLogroDTO?> Ejecutar(long id)
    {
        var logro = await _logroRepositorio.ObtenerLogroPorId(id);
        if (logro == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerLogroDTO>(logro);
    }
}
