using FitRank_API.Application.DTOs;
using FitRank_API.Infrastructure.Interfaces;
using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class ObtenerMaquinaDetalleCasoDeUso
    {
        private readonly IMaquinaRepositorio _maquinaRepositorio;
        private readonly IEjercicioRepositorio _ejercicioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerMaquinaDetalleCasoDeUso(
            IMaquinaRepositorio maquinaRepositorio,
            IEjercicioRepositorio ejercicioRepositorio,
            IMapper mapper)
        {
            _maquinaRepositorio = maquinaRepositorio;
            _ejercicioRepositorio = ejercicioRepositorio;
            _mapper = mapper;
        }

        public async Task<MaquinaDetalleDTO> Ejecutar(long maquinaId)
        {
            var maquina = await _maquinaRepositorio.ObtenerMaquinaPorId(maquinaId);

            if (maquina == null)
                throw new Exception("La máquina no existe.");

            var ejercicios = await _ejercicioRepositorio.ObtenerPorMaquinaId(maquinaId);

            return new MaquinaDetalleDTO
            {
                Id = maquina.Id,
                Nombre = maquina.Nombre,
                UrlImagen = maquina.UrlImagen,
                Qr = maquina.Qr,

                Ejercicios = ejercicios.Select(e => new EjercicioDeMaquinaDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Descripcion = e.Descripcion,
                    DuracionEstimada = e.DuracionEstimada,
                    UrlVideo = e.UrlVideo,
                    UrlImagen = e.UrlImagen,
                    GrupoMuscular = e.GrupoMuscular?.Nombre
                }).ToList()
            };
        }
    }
}
