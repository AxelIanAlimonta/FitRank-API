using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieCasosDeUso;

public class AgregarSerieCasoDeUso
{
    private readonly ISerieRepositorio _serieRepo;
    private readonly IMapper _mapper;

    public AgregarSerieCasoDeUso(ISerieRepositorio serieRepo, IMapper mapper)
    {
        _serieRepo = serieRepo;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerSerieDTO> Ejecutar(AgregarSerieDTO dto)
    {
        var nueva = _mapper.Map<Serie>(dto);
        var creada = await _serieRepo.AgregarAsync(nueva);
        return _mapper.Map<ObtenerSerieDTO>(creada);
    }
}
