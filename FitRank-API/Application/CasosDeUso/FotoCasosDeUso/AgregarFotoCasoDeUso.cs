using AutoMapper;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.FotoCasosDeUso
{
    public class AgregarFotoCasoDeUso
    {
        private readonly IFotoRepositorio _fotoRepositorio;
        private readonly IMapper _mapper;

        public AgregarFotoCasoDeUso(IFotoRepositorio fotoRepositorio, IMapper mapper)
        {
            _fotoRepositorio = fotoRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerFotoDTO> Ejecutar(AgregarFotoDTO dto)
        {
            var foto = _mapper.Map<Foto>(dto);
            await _fotoRepositorio.AgregarAsync(foto);
            return _mapper.Map<ObtenerFotoDTO>(foto);
        }
    }
}
