using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasosDeUso
{
    public class ObtenerSocioConMedidasCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly IMedidaCorporalRepositorio _medidaRepositorio;

        public ObtenerSocioConMedidasCasoDeUso(
            ISocioRepositorio socioRepositorio,
            IMedidaCorporalRepositorio medidaRepositorio)
        {
            _socioRepositorio = socioRepositorio;
            _medidaRepositorio = medidaRepositorio;
        }

        public async Task<SocioConMedidasDTO?> Ejecutar(long socioId)
        {
            var socio = await _socioRepositorio.ObtenerSocioYUsuarioPorIdAsync(socioId);
            if (socio == null) return null;

            var medida = await _medidaRepositorio.ObtenerUltimaMedidaPorSocioAsync(socioId);

            var socioDto = new SocioDTO
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Dni = socio.Dni,
                NombreUsuario = socio.NombreUsuario,
                Sexo = socio.Sexo,
                FotoUrl = socio.FotoDePerfil,
                CuotaPagadaHasta = socio.CuotaPagadaHasta,
                Altura = socio.Altura,
                Peso = socio.Peso,
                Nivel = socio.Nivel,
                Puntaje = socio.Puntaje,
                ParticipaEnRanking = socio.ParticipaEnRanking,
                GimnasioId = socio.GimnasioId,
                GimnasioNombre = socio.Gimnasio?.Nombre,
                QrToken = socio.QrToken
            };

            var medidaDto = medida == null ? null : new MedidaCorporalDTO
            {
                Id = medida.Id,
                Fecha = medida.Fecha,
                PechoCm = medida.PechoCm,
                CinturaCm = medida.CinturaCm,
                CaderaCm = medida.CaderaCm,
                BrazoDerechoCm = medida.BrazoDerechoCm,
                BrazoIzquierdoCm = medida.BrazoIzquierdoCm,
                PesoKg = medida.PesoKg
            };

            return new SocioConMedidasDTO
            {
                Socio = socioDto,
                UltimaMedida = medidaDto
            };
        }
    }
}
