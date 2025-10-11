using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizado;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Strategy;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.Services
{
    public class EjercicioRealizadoService : IEjercicioRealizado
    {
        private readonly IEjercicioRealizadoRepository _ejercicioRealizadoRepository;
        private readonly IPuntuacionDiariaRepository _puntuacionDiariaRepository;
        private readonly FitRankDbContext _context;
        private readonly IMapper _mapper;


        public EjercicioRealizadoService(
            IEjercicioRealizadoRepository ejercicioRealizadoRepository,
            IPuntuacionDiariaRepository puntuacionDiariaRepository,
            FitRankDbContext context,
            IMapper mapper)
        {
            _ejercicioRealizadoRepository = ejercicioRealizadoRepository;
            _puntuacionDiariaRepository = puntuacionDiariaRepository;
            _context = context;
            _mapper = mapper;
        }

        //metodos

        public async Task<IEnumerable<EjercicioRealizadoDTOSalida>> GetByUsuarioAsync(int usuarioId)
        {
            var ejercicios = await _context.EjerciciosRealizados
                .Include(e => e.Ejercicio)
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();

            return ConvertToEjercicioRealizadoDTOSalida(ejercicios);
        }

      

        public async Task<EjercicioRealizadoDTOSalida> RegistrarEjercicioAsync(EjercicioRealizadoDTOEntrada dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.EjerciciosRealizados)
                .ThenInclude(er => er.Ejercicio)
                .FirstOrDefaultAsync(u => u.Id == dto.UsuarioId);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            var ejercicio = await _context.Ejercicios.FindAsync(dto.EjercicioId);
            if (ejercicio == null)
                throw new Exception("Ejercicio no encontrado");

            bool puedeSumar = await puedeSumarPuntos(usuario, ejercicio);
            string mensaje;
            double puntos = 0;

            if (puedeSumar)
            {
                double factorDificultad = await _context.ConfiguracionesDificultad
                .Where(c => c.Dificultad == ejercicio.Dificultad)
                .Select(c => c.Multiplicador)
                .FirstOrDefaultAsync();

                var configGrupo = await _context.ConfiguracionesGrupoMuscular
                .Where(c => c.GrupoMuscular == ejercicio.GrupoMuscular)
                .Select(c => new { c.MultiplicadorPeso, c.MultiplicadorRepeticiones })
                .FirstOrDefaultAsync();

                double multiplicadorPeso = configGrupo?.MultiplicadorPeso ?? 0.1;
                double multiplicadorReps = configGrupo?.MultiplicadorRepeticiones ?? 0.1;


                var resultado = CalcularPuntosEjercicio(dto, usuario, ejercicio, factorDificultad, multiplicadorPeso, multiplicadorReps);

                puntos = resultado.Puntos;

                mensaje = $"Ejercicio registrado correctamente. Obtuviste {puntos:F2} puntos.";

                if (resultado.PesoAjustado)
                {
                    Console.WriteLine(resultado.MensajeAdvertencia);
                }
            }
            else
            {
                puntos = 0;
                mensaje = $"Ejercicio registrado, pero no sumaste puntos porque ya entrenaste {ejercicio.GrupoMuscular} más de dos veces esta semana.";
            }

            EjercicioRealizado ejercicioRealizado = ConstruirEjercicioRealizado(dto, puntos);
            _context.Entry(ejercicioRealizado).Reference(er => er.Ejercicio).IsModified = false;
            await _ejercicioRealizadoRepository.AddEjercicioRealizado(ejercicioRealizado);
            
            await GestionarPuntosDiariosAsync(dto, puntos);

            var divisionService = new CalculoDivisionService(_context);
            string nuevaDivision = await divisionService.CalcularDivisionAsync(usuario);
            usuario.Nivel = nuevaDivision;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();


            // Crear DTO de salida manualmente solo con los cálculos que querés mostrar
            var dtoSalida = new EjercicioRealizadoDTOSalida
            {
                PuntosObtenidos = ejercicioRealizado.PuntosObtenidos,

                FechaRegistro = ejercicioRealizado.FechaRegistro
            };

            return dtoSalida;
        }

        private static ResultadoCalculo CalcularPuntosEjercicio(EjercicioRealizadoDTOEntrada dto, Usuario usuario, Ejercicio ejercicio, double factorDificultad, double multiplicadorPeso, double multiplicadorReps)
        {
            

            double factorUsuario = (usuario.PesoKg / 70.0) * 0.5 + (usuario.AlturaCm / 175.0) * 0.5;


            //var estrategia = SeleccionDeCalculo.SeleccionarCalculo(ejercicio);
            var calculo =  new CalculoGenerico();
            var resultado = calculo.CalcularPuntos(
                ejercicio,
                dto.Series,
                dto.Repeticiones,
                dto.Peso,
                dto.TipoEntrenamiento,
                usuario,
                factorDificultad,
                factorUsuario,
                multiplicadorPeso,
                multiplicadorReps
            );
            return resultado;
        }

        private static EjercicioRealizado ConstruirEjercicioRealizado(EjercicioRealizadoDTOEntrada dto, double puntos)
        {
            // Mapeo DTO entrada → entidad
            return new EjercicioRealizado
            {
                UsuarioId = dto.UsuarioId,
                EjercicioId = dto.EjercicioId,
                Series = dto.Series,
                Repeticiones = dto.Repeticiones,
                Peso = dto.Peso,
                TipoDeEntrenamiento = dto.TipoEntrenamiento,
                ObservacionDelUsuario = dto.Observacion,
                FechaRegistro = DateTime.UtcNow,
                PuntosObtenidos = puntos


            };
        }

        private async Task GestionarPuntosDiariosAsync(EjercicioRealizadoDTOEntrada dto, double puntos)
        {

            // 🔹 Acumular puntos en PuntuacionDiaria
            var fechaHoy = DateTime.UtcNow.Date;
            var puntuacionDiaria = await _puntuacionDiariaRepository.GetByUsuarioYFechaAsync(dto.UsuarioId, fechaHoy);

            if (puntuacionDiaria == null)
            {
                puntuacionDiaria = new PuntuacionDiaria
                {
                    UsuarioId = dto.UsuarioId,
                    Fecha = fechaHoy,
                    Puntos = puntos
                };
                await _puntuacionDiariaRepository.RegistrarPuntuacionDiaria(puntuacionDiaria);
            }
            else
            {
                puntuacionDiaria.Puntos += puntos;
                await _puntuacionDiariaRepository.ModificarPuntuacionDiaria(puntuacionDiaria);
            }
        }
        private static IEnumerable<EjercicioRealizadoDTOSalida> ConvertToEjercicioRealizadoDTOSalida(List<EjercicioRealizado> ejercicios)
        {
            return ejercicios.Select(er => new EjercicioRealizadoDTOSalida
            {
                Id = er.Id,
                UsuarioId = er.UsuarioId,
                EjercicioId = er.EjercicioId,
                NombreEjercicio = er.Ejercicio?.Nombre,
                GrupoMuscular = er.Ejercicio?.GrupoMuscular ?? Domain.Enums.GrupoMuscular.Pecho,
                Series = er.Series,
                Repeticiones = er.Repeticiones,
                Peso = er.Peso,
                PuntosObtenidos = er.PuntosObtenidos,
                FechaRegistro = er.FechaRegistro,
                Observacion = er.ObservacionDelUsuario
            });
        }

        private async Task<bool> puedeSumarPuntos(Usuario usuario, Ejercicio ejercicio)
        {
            var grupo = ejercicio.GrupoMuscular;

            var inicioSemana = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            var finSemana = inicioSemana.AddDays(6);

            var diasEntrenados = await _context.EjerciciosRealizados
                .Where(er => er.UsuarioId == usuario.Id &&
                             er.FechaRegistro >= inicioSemana &&
                             er.FechaRegistro <= finSemana &&
                             er.Ejercicio.GrupoMuscular == grupo)
                .Select(er => er.FechaRegistro.Date)
                .Distinct()
                .CountAsync();


            return diasEntrenados <= 2;
        }
    }



















}



