using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

public class ObtenerRutinaCompletaCasoDeUso
{
    private readonly IRutinaRepositorio _repo;
    private readonly IMapper _mapper;

    public ObtenerRutinaCompletaCasoDeUso(IRutinaRepositorio repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

   public async Task<List<RutinaCompletaDTO>> Ejecutar(long socioId)
        {
      
        var rutinas = await _repo.ObtenerRutinasPorSocioAsync(socioId);

       
        var resultado = rutinas.Select(r => new RutinaCompletaDTO
        {
            Id = r.Id,
            Nombre = r.Nombre,
            Descripcion = r.Descripcion ?? "",
            Activa = r.Activa,
            Sesiones = r.Sesiones?.Select(s => new SesionDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                NumeroDeSesion = s.NumeroDeSesion,

                EjerciciosAsignados = s.EjerciciosAsignados?.Select(ea => new EjercicioAsignadoDTO
                {
                    Id = ea.Id,
                    NumeroEjercicio = ea.NumeroEjercicio,

                    Ejercicio = new EjercicioDTO
                    {
                        Id = ea.Ejercicio.Id,
                        Nombre = ea.Ejercicio.Nombre,
                        Descripcion = ea.Ejercicio.Descripcion,
                        UrlImagen = ea.Ejercicio.UrlImagen,
                        UrlVideo = ea.Ejercicio.UrlVideo,
                        DuracionEstimada = ea.Ejercicio.DuracionEstimada ?? 0
                    },

                    Series = ea.Series?.Select(se => new SerieDTO
                    {
                        Id = se.Id,
                        Peso = se.Peso,
                        Repeticiones = se.Repeticiones,
                        Duracion = se.Duracion
                    }).ToList() ?? new()
                }).ToList() ?? new()
            }).ToList() ?? new()
        }).ToList();

        return resultado;
    }
}
