using AutoMapper;
using FitRank_API.Application.DTOs.SerieAsignadaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;

public class AgregarSerieAsignadaCasoDeUso
{
    private readonly ISerieAsignadaRepositorio _serieAsignadaRepositorio;
    private readonly IMapper _mapper;

    public AgregarSerieAsignadaCasoDeUso(ISerieAsignadaRepositorio serieAsignadaRepositorio, IMapper mapper)
    {
        _serieAsignadaRepositorio = serieAsignadaRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerSerieAsignadaDTO> Ejecutar(AgregarSerieAsignadaDTO agregarSerieAsignadaDTO)
    {
        var serieAsignadaEntidad = _mapper.Map<Domain.Entities.SerieAsignada>(agregarSerieAsignadaDTO);
        var serieAsignadaCreada = await _serieAsignadaRepositorio.AgregarAsync(serieAsignadaEntidad);
        return _mapper.Map<ObtenerSerieAsignadaDTO>(serieAsignadaCreada);

    }
}
