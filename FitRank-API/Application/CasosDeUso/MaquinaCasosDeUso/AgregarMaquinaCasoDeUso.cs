using AutoMapper;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

public class AgregarMaquinaCasoDeUso
{
    private readonly IMaquinaRepositorio _maquinaRepositorio;
    private readonly IMapper _mapper;
    private readonly QrHelper _qrHelper;

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
    
        maquina = await _maquinaRepositorio.AgregarMaquina(maquina);

   
        maquina.Qr = await _qrHelper.GenerarQrDeMaquina(maquina.Id);

      
        await _maquinaRepositorio.ActualizarMaquina(maquina);

        return _mapper.Map<ObtenerMaquinaDTO>(maquina);
    }
}
