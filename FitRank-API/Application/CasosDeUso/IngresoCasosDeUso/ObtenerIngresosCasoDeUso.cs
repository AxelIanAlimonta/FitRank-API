using AutoMapper;

using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.Ingreso
{
    public class ObtenerIngresosCasoDeUso
    {
        private readonly IIngresoRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerIngresosCasoDeUso(IIngresoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<ObtenerIngresoDTO>> Ejecutar()
        {
            var ingresos = await _repo.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<ObtenerIngresoDTO>>(ingresos);
        }
    }
}
