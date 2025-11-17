using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroCasosDeUso;

public class AgregarLogroCasoDeUso
{
    private readonly ILogroRepositorio _logroRepositorio;
    private readonly IMapper _mapper;
    public AgregarLogroCasoDeUso(ILogroRepositorio logroRepositorio, IMapper mapper)
    {
        _logroRepositorio = logroRepositorio;
        _mapper = mapper;
    }
    public virtual async Task<ObtenerLogroDTO> Ejecutar(AgregarLogroDTO crearLogroDTO)
    {
        var logroEntidad = _mapper.Map<Logro>(crearLogroDTO);
        var logroCreado = await _logroRepositorio.AgregarLogro(logroEntidad);
        return _mapper.Map<ObtenerLogroDTO>(logroCreado);
    }

}
