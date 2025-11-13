using AutoMapper;

using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.Ingreso
{
    public class ObtenerIngresoPorIdCasoDeUso
    {
        private readonly IIngresoRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerIngresoPorIdCasoDeUso(IIngresoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ObtenerIngresoDTO?> Ejecutar(long id)
        {
            
            var ingreso = await _repo.ObtenerPorIdAsync(id);

            
            if (ingreso == null)
                return null;

            
            return _mapper.Map<ObtenerIngresoDTO>(ingreso);
        }
    }
}
