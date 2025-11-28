using AutoMapper;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.FotoCasosDeUso
{
    public class ObtenerFotosPorSocioCasoDeUso
    {
        private readonly IFotoRepositorio _fotoRepositorio;
        private readonly IMapper _mapper;

        public ObtenerFotosPorSocioCasoDeUso(IFotoRepositorio fotoRepositorio, IMapper mapper)
        {
            _fotoRepositorio = fotoRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<ObtenerFotoDTO>> Ejecutar(long socioId)
        {
            var fotos = await _fotoRepositorio.ObtenerPorSocioAsync(socioId);
            return _mapper.Map<List<ObtenerFotoDTO>>(fotos);
        }
    }
}
