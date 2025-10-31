using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Serie
{
    public class AgregarSerieCasoDeUso
    {
        private readonly ISerieRepositorio _serieRepo;
        private readonly IMapper _mapper;

        public AgregarSerieCasoDeUso(ISerieRepositorio serieRepo, IMapper mapper)
        {
            _serieRepo = serieRepo;
            _mapper = mapper;
        }

        public async Task<ObtenerSerieDTO> Ejecutar(AgregarSerieDTO dto)
        {
            var nueva = _mapper.Map<Domain.Entities.Serie>(dto);
            var creada = await _serieRepo.AgregarAsync(nueva);
            return _mapper.Map<ObtenerSerieDTO>(creada);
        }
    }
}
