using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

using FitRank_API.Application.DTOs.IngresoDTOs;

namespace FitRank_API.Application.CasosDeUso.Ingreso
{
    public class AgregarIngresoCasoDeUso
    {
        private readonly IIngresoRepositorio _repo;
        private readonly IMapper _mapper;

        public AgregarIngresoCasoDeUso(IIngresoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerIngresoDTO> Ejecutar(AgregarIngresoDTO dto)
        {
            var ingreso = _mapper.Map<Domain.Entities.Ingreso>(dto);
            ingreso.Fecha = DateTime.UtcNow;
            ingreso.Confirmado = true;

            await _repo.AgregarAsync(ingreso);
            await _repo.GuardarCambiosAsync();

            return _mapper.Map<ObtenerIngresoDTO>(ingreso);
        }
    }
}
