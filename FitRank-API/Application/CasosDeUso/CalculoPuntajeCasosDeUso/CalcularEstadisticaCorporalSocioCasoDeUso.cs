using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Application.DTOs.CalcularPuntajeDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases
{
    public class CalcularEstadisticaCorporalSocioCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly FitRankDbContext _context;

        public CalcularEstadisticaCorporalSocioCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        public async Task<EstadisticaCorporalSocioDTO?> Ejecutar(long socioId)
        {
            var socio = await _socioRepositorio.ObtenerSocioConMedidasAsync(socioId);

            if (socio == null || socio.MedidasCorporales == null || !socio.MedidasCorporales.Any())
                return null;

            // Tomamos la última medida
            var ultimaMedida = socio.MedidasCorporales
                .OrderByDescending(m => m.Fecha)
                .First();

            if (ultimaMedida == null)
            {
                throw new Exception("El socio no tiene medidas corporales registradas.");
            }

            double imc = ultimaMedida.PesoKg / Math.Pow(socio.Altura, 2);
            string clasificacion = ClasificarImc(imc);

            return new EstadisticaCorporalSocioDTO
            {
                Imc = Math.Round(imc, 2),
                ClasificacionImc = clasificacion,
                Peso = ultimaMedida.PesoKg,
                Altura = socio.Altura,
                FechaMedicion = ultimaMedida.Fecha
            };
        }

        private string ClasificarImc(double imc)
        {
            if (imc < 18.5) return "Bajo peso";
            if (imc < 25) return "Normal";
            if (imc < 30) return "Sobrepeso";
            return "Obesidad";
        }
    }
}
