using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Enums;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AmistadCasosDeUso
{
    public class EliminarAmigoCasoDeUso
    {
        private readonly IAmistadRepositorio _amistadRepositorio;

        public EliminarAmigoCasoDeUso(IAmistadRepositorio amistadRepositorio)
        {
            _amistadRepositorio = amistadRepositorio;
        }

        public virtual async Task<bool> Ejecutar(EliminarAmigoDTO dto)
        {
            if (dto.SocioId == dto.AmigoId)
                return false;

            var socioId1 = Math.Min(dto.SocioId, dto.AmigoId);
            var socioId2 = Math.Max(dto.SocioId, dto.AmigoId);

            var amistad = await _amistadRepositorio.ObtenerPorIdDeSociosAsync(socioId1, socioId2);

            if (amistad == null || amistad.Estado != EstadoAmistad.Aceptado)
                return false;

            await _amistadRepositorio.EliminarAsync(amistad);
            return true;
        }
    }
}
