using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso
{
    public class ActualizarPersonalizacionGimnasioCasoDeUso
    {
        private readonly IGimnasioRepositorio _gimnasioRepositorio;
        private readonly IMapper _mapper;

        public ActualizarPersonalizacionGimnasioCasoDeUso(
            IGimnasioRepositorio gimnasioRepositorio,
            IMapper mapper)
        {
            _gimnasioRepositorio = gimnasioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerGimnasioDTO?> Ejecutar(ActualizarPersonalizacionDTO dto)
        {
            var gymActualizado = await _gimnasioRepositorio.ActualizarPersonalizacion(
                dto.Id,
                dto.ColorPrincipal,
                dto.ColorSecundario,
                dto.LogoUrl
            );

            if (gymActualizado == null)
                return null;

            return _mapper.Map<ObtenerGimnasioDTO>(gymActualizado);
        }
    }
}
