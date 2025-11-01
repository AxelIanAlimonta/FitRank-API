using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using System.Text.Json;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class ConfirmarRutinaIACasoDeUso
    {
        private readonly IRutinaRepositorio _rutinaRepositorio;

        public ConfirmarRutinaIACasoDeUso(IRutinaRepositorio rutinaRepositorio)
        {
            _rutinaRepositorio = rutinaRepositorio;
        }

        public async Task<ResultadoConfirmarRutinaDTO> EjecutarAsync(ConfirmarRutinaDTO body)
        {
            if (body is null || body.Rutina is null)
                return ResultadoConfirmarRutinaDTO.Fallo("Body vacío.");

            // Validar socio y ejercicios dentro del repositorio
            var validacion = await _rutinaRepositorio.ValidarReferenciasAsync(body);
            if (!validacion.Ok)
                return ResultadoConfirmarRutinaDTO.Fallo(validacion.Mensaje);

            // Convertir snapshots
            JsonDocument? snapDoc = null;
            JsonDocument? rulesDoc = null;
            try
            {
                if (body.Rutina.InputSnapshot is not null)
                    snapDoc = JsonDocument.Parse(JsonSerializer.Serialize(body.Rutina.InputSnapshot));
                if (body.Rutina.RulesExplain is not null)
                    rulesDoc = JsonDocument.Parse(JsonSerializer.Serialize(body.Rutina.RulesExplain));
            }
            catch { }

            // Crear entidad de Rutina
            var rutina = new Rutina
            {
                Nombre = body.Rutina.Nombre,
                TipoCreacion = "IA",
                FechaCreacion = DateTime.UtcNow,
                Descripcion = $"{body.Rutina.Objetivo} · {body.Rutina.Division}",
                Activa = true,
                SocioId = body.SocioId,
                UsuarioId = body.UsuarioId,
                InputSnapshotJson = snapDoc,
                RulesExplainJson = rulesDoc,
                Sesiones = new List<Sesion>()
            };

            // Guardar usando el repositorio
            await _rutinaRepositorio.GuardarRutinaCompletaAsync(rutina, body.Rutina.SesionesPlan);

            return ResultadoConfirmarRutinaDTO.Exito(rutina.Id);
        }
    }
}
