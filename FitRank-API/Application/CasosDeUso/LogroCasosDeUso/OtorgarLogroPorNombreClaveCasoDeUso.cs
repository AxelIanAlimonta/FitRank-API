using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroCasosDeUso;

public class OtorgarLogroPorNombreClaveCasoDeUso
{
    private readonly ILogroRepositorio _logroRepositorio;
    private readonly ILogroGimnasioRepositorio _logroGimnasioRepositorio;
    private readonly ILogroSocioRepositorio _logroSocioRepositorio;
    private readonly IMapper _mapper;

    public OtorgarLogroPorNombreClaveCasoDeUso(
        ILogroRepositorio logroRepositorio,
        ILogroGimnasioRepositorio logroGimnasioRepositorio,
        ILogroSocioRepositorio logroSocioRepositorio,
        IMapper mapper)
    {
        _logroRepositorio = logroRepositorio;
        _logroGimnasioRepositorio = logroGimnasioRepositorio;
        _logroSocioRepositorio = logroSocioRepositorio;
        _mapper = mapper;
    }

    public async Task<LogroOtorgadoDTO> Ejecutar(OtorgarLogroPorNombreClaveDTO dto)
    {
        // DTO base: siempre devolvemos socio + gym
        var salida = new LogroOtorgadoDTO
        {
            SocioId = dto.SocioId,
            GimnasioId = dto.GimnasioId,
            Otorgado = false
        };

        // 1. Buscar logro global
        var logro = await _logroRepositorio.ObtenerPorNombreClaveAsync(dto.NombreClave);

        if (logro is null || !logro.Estado)
        {
            salida.Motivo = "El logro no existe o está desactivado globalmente.";
            return salida;
        }

        salida.LogroId = logro.Id;
        salida.Nombre = logro.Nombre;
        salida.NombreClave = logro.NombreClave;

        // 2. Verificar config del gimnasio
        var logroGimnasio = await _logroGimnasioRepositorio
            .ObtenerPorGimnasioYLogroAsync(dto.GimnasioId, logro.Id);

        if (logroGimnasio is null || !logroGimnasio.EstaActivo)
        {
            salida.Motivo = "El gimnasio no tiene habilitado este logro.";
            return salida;
        }

        // 3. Verificar si el socio ya lo tiene
        var yaLoTiene = await _logroSocioRepositorio
            .ExisteAsync(logro.Id, dto.GimnasioId, dto.SocioId);

        if (yaLoTiene)
        {
            salida.Motivo = "El socio ya tiene este logro.";
            return salida;
        }

        // 4. Crear el LogroSocio (ACÁ recién se otorga)
        var logroSocio = new LogroSocio
        {
            LogroId = logro.Id,
            GimnasioId = dto.GimnasioId,
            SocioId = dto.SocioId,
            FechaObtenido = DateTime.UtcNow
        };

        var creado = await _logroSocioRepositorio.CrearAsync(logroSocio);

        // 5. Completar salida
        salida.Otorgado = true;
        salida.FechaOtorgado = creado.FechaObtenido;
        salida.Motivo = null;

        return salida;
    }
}
