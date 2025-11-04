using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso
{
    public class CrearSolicitudRutinaProfesorCasoDeUso
    {
        private readonly ISolicitudRutinaProfesorRepositorio _repositorio;

        public CrearSolicitudRutinaProfesorCasoDeUso(ISolicitudRutinaProfesorRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<long> EjecutarAsync(CrearSolicitudRutinaProfesorDTO dto, long socioId)
        {
            var solicitud = new SolicitudRutinaProfesor
            {
                SocioId = socioId,
                MensajeSocio = dto.MensajeSocio,
                Edad = dto.Edad,
                PesoKg = dto.PesoKg,
                AlturaCm = dto.AlturaCm,
                Nivel = dto.Nivel,
                SesionesPorSemana = dto.SesionesPorSemana,
                MinutosPorSesion = dto.MinutosPorSesion,
                Objetivo = dto.Objetivo,
                CalidadAlimentacion = dto.CalidadAlimentacion,
                HorasSuenio = dto.HorasSuenio,
                DolorLumbar = dto.DolorLumbar,
                DolorRodilla = dto.DolorRodilla,
                DolorHombro = dto.DolorHombro,
                CirugiaReciente = dto.CirugiaReciente,
                Sincope = dto.Sincope,
                Embarazo = dto.Embarazo,
                Hipertension = dto.Hipertension,
                HipertensionControlada = dto.HipertensionControlada,
                Diabetes = dto.Diabetes,
                DolorToracico = dto.DolorToracico,
                FrecuenciaCardiacaReposo = dto.FrecuenciaCardiacaReposo,
                Estado = EstadoSolicitud.Pendiente
            };

            await _repositorio.AgregarAsync(solicitud);
            return solicitud.Id;
        }
    }

}
