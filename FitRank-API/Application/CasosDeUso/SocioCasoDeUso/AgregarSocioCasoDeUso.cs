using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class AgregarSocioCasoDeUso
{
    private readonly ISocioRepositorio _socioRepositorio;
    private readonly IMapper _mapper;

    public AgregarSocioCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper)
    {
        _socioRepositorio = socioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<SocioDTO> Ejecutar(AgregarSocioDTO socio)
    {
       
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(socio.Password);

     
        var socioEntidad = _mapper.Map<Socio>(socio);

        socioEntidad.PasswordHash = hashedPassword;
        socioEntidad.EsActivado = true;   
        socioEntidad.Rol = "Socio";       

        var socioCreado = await _socioRepositorio.AgregarAsync(socioEntidad);

        return _mapper.Map<SocioDTO>(socioCreado);

    }

}
