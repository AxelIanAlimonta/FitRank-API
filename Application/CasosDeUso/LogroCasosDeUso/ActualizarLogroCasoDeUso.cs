using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroCasosDeUso;

public class ActualizarLogroCasoDeUso
{
    private readonly ILogroRepositorio _logroRepositorio;
    private readonly IMapper _mapper;
    public ActualizarLogroCasoDeUso(ILogroRepositorio logroRepositorio, IMapper mapper)
    {
        _logroRepositorio = logroRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerLogroDTO?> Ejecutar(ActualizarLogroDTO dto)
    {
        var logroAActualizar = _mapper.Map<Logro>(dto);
        var logroActualizado = await _logroRepositorio.ActualizarLogro(logroAActualizar);
        return _mapper.Map<ObtenerLogroDTO>(logroActualizado);
    }

}
