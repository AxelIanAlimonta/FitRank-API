using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class LogroProfile : Profile
{
    public LogroProfile()
    {

        CreateMap<AgregarLogroDTO, Logro>();

        CreateMap<ActualizarLogroDTO, Logro>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            {
                if (srcMember == null) return false;
                return srcMember is string s ? !string.IsNullOrWhiteSpace(s) : true;
            }));

        CreateMap<Logro, ObtenerLogroDTO>();


        CreateMap<LogroGimnasio, LogroGimnasioDTO>()
            .ForMember(d => d.GimnasioId, o => o.MapFrom(s => s.GimnasioId))
            .ForMember(d => d.LogroId, o => o.MapFrom(s => s.LogroId))
            .ForMember(d => d.EstaHabilitado, o => o.MapFrom(s => s.EstaActivo))
            .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Logro.Nombre))
            .ForMember(d => d.NombreClave, o => o.MapFrom(s => s.Logro.NombreClave))
            .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Logro.Descripcion))
            .ForMember(d => d.Imagen, o => o.MapFrom(s => s.Logro.Imagen));

        CreateMap<ActualizarLogroGimnasioDTO, LogroGimnasio>();


        CreateMap<LogroSocio, LogroSocioDTO>()
            .ForMember(d => d.LogroId, o => o.MapFrom(s => s.LogroId))
            .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Logro.Nombre))
            .ForMember(d => d.NombreClave, o => o.MapFrom(s => s.Logro.NombreClave))
            .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Logro.Descripcion))
            .ForMember(d => d.Imagen, o => o.MapFrom(s => s.Logro.Imagen))
            .ForMember(d => d.FechaOtorgado, o => o.MapFrom(s => s.FechaObtenido));


        CreateMap<LogroSocio, LogroOtorgadoDTO>()
            .ForMember(d => d.LogroId, o => o.MapFrom(s => s.LogroId))
            .ForMember(d => d.SocioId, o => o.MapFrom(s => s.SocioId))
            .ForMember(d => d.GimnasioId, o => o.MapFrom(s => s.GimnasioId))
            .ForMember(d => d.FechaOtorgado, o => o.MapFrom(s => s.FechaObtenido))
            .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Logro.Nombre))
            .ForMember(d => d.NombreClave, o => o.MapFrom(s => s.Logro.NombreClave))
            .ForMember(d => d.Otorgado, o => o.Ignore()) 
            .ForMember(d => d.Motivo, o => o.Ignore());  
    }
}
