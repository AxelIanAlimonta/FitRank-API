using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;

public class AgregarGimnasioCasoDeUso
{
    private readonly IGimnasioRepositorio _gimnasioRepositorio;
    private readonly IMapper _mapper;
    private readonly IAdministradorRepositorio _adminRepositorio;
  
  public AgregarGimnasioCasoDeUso(IGimnasioRepositorio gimnasioRepositorio, IMapper mapper, IAdministradorRepositorio adminRepositorio)
    {
        _gimnasioRepositorio = gimnasioRepositorio;
        _mapper = mapper;
        _adminRepositorio = adminRepositorio;
    }


    public async Task<ObtenerGimnasioDTO> Ejecutar(AgregarGimnasioDTO dto)
    {
        // 1️⃣ Validar si el admin existe (si viene)
        Administrador? admin = null;
        if (dto.AdministradorId.HasValue)
        {
            admin = await _adminRepositorio.ObtenerPorIdAsync(dto.AdministradorId.Value);
            if (admin == null)
                throw new Exception("No se encontró el administrador indicado.");

        }

        // 2️⃣ Mapear y asignar admin (si aplica)
        var gimnasioEntidad = _mapper.Map<Gimnasio>(dto);
        gimnasioEntidad.AdministradorId = admin?.Id;

        // 3️⃣ Guardar
        var gimnasioCreado = await _gimnasioRepositorio.AgregarGimnasio(gimnasioEntidad);

        // 4️⃣ Devolver DTO de salida
        return _mapper.Map<ObtenerGimnasioDTO>(gimnasioCreado);
    }
}


