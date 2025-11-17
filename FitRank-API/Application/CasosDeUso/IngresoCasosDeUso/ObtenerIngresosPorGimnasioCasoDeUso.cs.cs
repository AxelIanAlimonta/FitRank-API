using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

using FitRank_API.Application.DTOs.IngresoDTOs;

namespace FitRank_API.Application.CasosDeUso.Ingreso
{
    public class ObtenerIngresosPorGimnasioCasoDeUso
    {
        private readonly IIngresoRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerIngresosPorGimnasioCasoDeUso(IIngresoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ObtenerIngresoDTO>> Ejecutar(long gimnasioId)
        {
            var ingresos = await _repo.ObtenerPorGimnasioAsync(gimnasioId);
            return _mapper.Map<IEnumerable<ObtenerIngresoDTO>>(ingresos);
        }
    }
}
