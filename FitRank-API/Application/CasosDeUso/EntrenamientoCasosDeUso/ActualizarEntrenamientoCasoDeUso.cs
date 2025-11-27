using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class ActualizarEntrenamientoCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;
        private readonly IMapper _mapper;

        public ActualizarEntrenamientoCasoDeUso(IEntrenamientoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerEntrenamientoDTO?> Ejecutar(ActualizarEntrenamientoDTO dto)
        {
            var entrenamientoExistente = await _repo.ObtenerPorIdAsync(dto.Id);
            if (entrenamientoExistente == null)
            {
                return null;
            }

            entrenamientoExistente.Fecha = dto.Fecha;
            entrenamientoExistente.Duracion = dto.Duracion;
            entrenamientoExistente.SocioId = dto.SocioId;

            var entrenamientoActualizado = await _repo.ActualizarAsync(entrenamientoExistente);
            return _mapper.Map<ObtenerEntrenamientoDTO>(entrenamientoActualizado);
        }
    }
}
