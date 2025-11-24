using AutoMapper;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso
{
    public class ObtenerAdministradorCasoDeUso
    {
        private readonly IAdministradorRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerAdministradorCasoDeUso(IAdministradorRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<ObtenerAdministradorDTO>> Ejecutar()
        {
            var admins = await _repo.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<ObtenerAdministradorDTO>>(admins);
        }
    }
}