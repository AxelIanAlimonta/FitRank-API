using AutoMapper;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AmistadCasosDeUso
{
    public class ObtenerAmigosCasoDeUso
    {
        private readonly IAmistadRepositorio _amistadRepositorio;
        private readonly IMapper _mapper;

        public ObtenerAmigosCasoDeUso(
            IAmistadRepositorio amistadRepositorio,
            IMapper mapper)
        {
            _amistadRepositorio = amistadRepositorio;
            _mapper = mapper;
        }

        public async Task<List<AmigoDTO>> Ejecutar(long socioId)
        {
            var amistades = await _amistadRepositorio.ObtenerPorSocioIdAsync(
                socioId,
                EstadoAmistad.Aceptado);

            var amigosSocios = amistades
                .Select(a => a.SocioId1 == socioId ? a.Socio2 : a.Socio1)
                .ToList();

            return _mapper.Map<List<AmigoDTO>>(amigosSocios);
        }
    }
}
