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

    public async Task<ObtenerLogroDTO?> Ejecutar(long id, ActualizarLogroDTO dto)
    {
        var logroExistente = await _logroRepositorio.ObtenerLogroPorId(id);
        if (logroExistente == null)
        {
            return null;
        }

        _mapper.Map(dto, logroExistente);
        var logroActualizado = await _logroRepositorio.ActualizarLogro(logroExistente);
        if (logroActualizado is null) return null;

        return _mapper.Map<ObtenerLogroDTO>(logroActualizado);
    }

}
