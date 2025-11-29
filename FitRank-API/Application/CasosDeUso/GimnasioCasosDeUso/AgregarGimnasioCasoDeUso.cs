using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class AgregarGimnasioCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    private readonly IMapper _mapper;
    private readonly IAdministradorRepositorio _adminRepositorio;

    public AgregarGimnasioCasoDeUso(IGimnasioRepositorio gimnasioRepositorio,
            IMapper mapper,
            IAdministradorRepositorio adminRepositorio)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
        _mapper = mapper;
        _adminRepositorio = adminRepositorio;
    }



    public virtual async Task<ObtenerGimnasioDTO> Ejecutar(AgregarGimnasioDTO dto)
    {
        Administrador? admin = null;
        if (dto.AdministradorId.HasValue)
        {
            admin = await _adminRepositorio.ObtenerPorIdAsync(dto.AdministradorId.Value);
            if (admin == null)
                throw new Exception("No se encontró el administrador indicado.");

        }

        var gimnasioEntidad = _mapper.Map<Gimnasio>(dto);
        gimnasioEntidad.AdministradorId = admin?.Id;

        var gimnasioCreado = await _gimnasioRepositorio.AgregarGimnasio(gimnasioEntidad);

        if (admin != null)
        {
            admin.GimnasioId = gimnasioCreado.Id;
            await _adminRepositorio.ActualizarAsync(admin);
        }

        return _mapper.Map<ObtenerGimnasioDTO>(gimnasioCreado);
    }
}


