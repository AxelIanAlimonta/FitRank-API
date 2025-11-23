using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class EntrenamientoProfile : Profile
{
    public EntrenamientoProfile()
    {
        CreateMap<AgregarEntrenamientoDTO, Entrenamiento>().ReverseMap();
        CreateMap<ActualizarEntrenamientoDTO, Entrenamiento>().ReverseMap();
        CreateMap<Entrenamiento, ObtenerEntrenamientoDTO>().ReverseMap();


        // Entrenamiento → EntrenamientoHistorialDTO
        CreateMap<Entrenamiento, EntrenamientoHistorialDTO>()
            .ForMember(dest => dest.IdEntrenamiento, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NombreSesion, opt => opt.MapFrom((src, dest) =>
            {
                var act = src.Actividades.FirstOrDefault();
                return act?.EjercicioAsignado?.Sesion?.Nombre ?? "Sesión sin nombre";
            }))
            .ForMember(dest => dest.NombreRutina,
                opt => opt.MapFrom(src =>
                    src.Actividades
                        .OrderBy(a => a.Id)
                        .Select(a => a.EjercicioAsignado.Sesion.Rutina.Nombre)
                        .FirstOrDefault() ?? "Sin rutina"
            ))
            .ForMember(dest => dest.NombreSocio,
                opt => opt.MapFrom(src => src.Socio.Nombre))
            .ForMember(dest => dest.PuntosTotales,
                opt => opt.MapFrom(src => src.Actividades.Sum(a => a.Punto ?? 0)));


        // Actividad → ActividadHistorialDTO
        CreateMap<Actividad, ActividadHistorialDTO>()
            .ForMember(dest => dest.IdActividad,
                opt => opt.MapFrom(src => src.Id)) // <--- ESTA ES LA CLAVE
            .ForMember(dest => dest.IdEjercicioAsignado,
                opt => opt.MapFrom(src => src.EjercicioAsignadoId))
            .ForMember(dest => dest.NombreEjercicio,
                opt => opt.MapFrom(src => src.EjercicioAsignado.Ejercicio.Nombre))
            .ForMember(dest => dest.UrlImagen,
                opt => opt.MapFrom(src => src.EjercicioAsignado.Ejercicio.UrlImagen))
            .ForMember(dest => dest.ProgresoHistorico, opt => opt.Ignore());


        // Construcción de Progreso Histórico REAL
        CreateMap<Actividad, ProgresoEjercicioDTO>()
            .ForMember(dest => dest.Fecha,
                opt => opt.MapFrom(src => src.Entrenamiento.Fecha))
            .ForMember(dest => dest.Peso,
                opt => opt.MapFrom(src => src.Peso))
            .ForMember(dest => dest.Repeticiones,
                opt => opt.MapFrom(src => src.Repeticiones));
    }
}
