using AutoMapper;
using FitRank_API.Application.DTOs.Rutina;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class RutinaServicioImpl : IRutinaServicio
    {
        private readonly IRutinaRepositorio _rutinaRepositorio;
        private readonly IMapper _mapper;

        public RutinaServicioImpl(IRutinaRepositorio rutinaRepositorio, IMapper mapper)
        {
            _rutinaRepositorio = rutinaRepositorio;
            _mapper = mapper;
        }
        public async Task<RutinaDTO> CrearRutinaAsync(CrearRutinaDTO dto)
        {
            //Mapeo el DTO a mi entity de dominio
            var rutina = _mapper.Map<Rutina>(dto);
            //Le proporcion a mi repositorio un objeto de mi dominio
            var rutinaCreada = await _rutinaRepositorio.CrearRutinaAsync(rutina);
            // retorno el mapeo de mi objeto de dominio a DTO
            return _mapper.Map<RutinaDTO>(rutinaCreada);
        }

        public async Task<RutinaDTO?> ObtenerRutinaAsync(int id)
        {
            var rutina = await _rutinaRepositorio.ObtenerRutinaAsync(id);

            if (rutina == null)
                return null;

            return _mapper.Map<RutinaDTO>(rutina);
        }

        public async Task<List<RutinaDTO>> ListarRutinasAsync(int idUsuario)
        {
            var rutinas = await _rutinaRepositorio.ListarRutinasAsync(idUsuario);
            return _mapper.Map<List<RutinaDTO>>(rutinas);
        }

        public async Task<RutinaDTO> ActualizarRutinaAsync(int id, ActualizarRutinaDTO dto)
        {
            var rutinaExistente = await _rutinaRepositorio.ObtenerRutinaAsync(id);
            if (rutinaExistente == null)
                return null;
            //mapea los datos del DTO a la rutina encontrada en la BDD, asi luego la guardo
            _mapper.Map(dto, rutinaExistente);

            var rutinaActualizada = await _rutinaRepositorio.ActualizarAsync(rutinaExistente);

            return _mapper.Map<RutinaDTO>(rutinaActualizada);
        }

        public async Task<bool> EliminarRutinaAsync(int id)
        {
            var rutina = await _rutinaRepositorio.ObtenerRutinaAsync(id);
            if (rutina == null)
                return false;

            await _rutinaRepositorio.EliminarRutinaAsync(rutina);
            return true;
        }
    }
}
