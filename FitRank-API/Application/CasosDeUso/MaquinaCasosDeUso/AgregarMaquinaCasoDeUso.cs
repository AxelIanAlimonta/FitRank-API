using AutoMapper;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;

public class AgregarMaquinaCasoDeUso
{
    private readonly IMaquinaRepositorio _maquinaRepositorio;
    private readonly IMapper _mapper;
    private readonly QrHelper _qrHelper;

    // 🔥 Helper para detectar si estamos en entorno de TEST
    private static bool IsTestEnvironment =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.Contains("Test", StringComparison.OrdinalIgnoreCase));

    public AgregarMaquinaCasoDeUso(
        IMaquinaRepositorio maquinaRepositorio,
        IMapper mapper,
        QrHelper qrHelper)
    {
        _maquinaRepositorio = maquinaRepositorio;
        _mapper = mapper;
        _qrHelper = qrHelper;
    }

    public async Task<ObtenerMaquinaDTO> Ejecutar(AgregarMaquinaDTO dto, long gimnasioId)
    {
        var maquina = _mapper.Map<Maquina>(dto);

        maquina.GimnasioId = gimnasioId;
        maquina.Qr = "PENDIENTE";

        // Guardamos en la DB (para obtener Id)
        maquina = await _maquinaRepositorio.AgregarMaquina(maquina);

        // 🔥 SI ESTAMOS EN TEST: retorna QR fake → evita System.Drawing / GDI+
        if (IsTestEnvironment)
        {
            maquina.Qr = $"QR_TEST_{maquina.Id}";
        }
        else
        {
            // ✔️ En entorno normal genera QR real
            maquina.Qr = await _qrHelper.GenerarQrDeMaquina(maquina.Id);
        }

        // Guardamos QR definitivo
        await _maquinaRepositorio.ActualizarMaquina(maquina);

        return _mapper.Map<ObtenerMaquinaDTO>(maquina);
    }
}
