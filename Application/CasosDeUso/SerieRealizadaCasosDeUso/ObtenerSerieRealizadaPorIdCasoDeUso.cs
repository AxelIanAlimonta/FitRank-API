namespace FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso
{
    public class ObtenerSerieRealizadaPorIdCasoDeUso
    {
        private readonly ISerieRealizadaRepositorio _serieRealizadaRepositorio;
        private readonly IMapper _mapper;
        public ObtenerSerieRealizadaPorIdCasoDeUso(ISerieRealizadaRepositorio serieRealizadaRepositorio, IMapper mapper)
        {
            _serieRealizadaRepositorio = serieRealizadaRepositorio;
            _mapper = mapper;
        }
        public async Task<ObtenerSerieRealizadaDTO?> Ejecutar(int idSerieRealizada)
        {
            var serieRealizadaEntidad = await _serieRealizadaRepositorio.ObtenerPorId(idSerieRealizada);
            if (serieRealizadaEntidad == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerSerieRealizadaDTO>(serieRealizadaEntidad);
        }
    }
}
