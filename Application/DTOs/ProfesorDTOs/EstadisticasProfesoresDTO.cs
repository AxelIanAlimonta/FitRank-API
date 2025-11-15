namespace FitRank_API.Application.DTOs.ProfesorDTOs
{
    public class EstadisticasProfesoresDTO
    {
        public TopSolicitadoDTO? TopSolicitado { get; set; }
        public TopPendientesDTO? TopPendientes { get; set; }
        public TopCumplidorDTO? TopCumplidor { get; set; }
        public TopValoradaDTO? TopValorado { get; set; }
    }

    public class TopSolicitadoDTO
    {
        public string NombreProfesor { get; set; } = string.Empty;
        public int CantidadSolicitudes { get; set; }
    }

    public class TopPendientesDTO
    {
        public string NombreProfesor { get; set; } = string.Empty;
        public int Pendientes { get; set; }
    }

    public class TopCumplidorDTO
    {
        public string NombreProfesor { get; set; } = string.Empty;
        public int Completadas { get; set; }
    }

    public class TopValoradaDTO
    {
        public string NombreProfesor { get; set; } = string.Empty;
        public double PromedioValoracion { get; set; }
    }


}
