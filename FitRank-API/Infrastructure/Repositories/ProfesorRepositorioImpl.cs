using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class ProfesorRepositorioImpl : IProfesorRepositorio
    {
        private readonly FitRankDbContext _context;

        public ProfesorRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Profesor>> ObtenerTodosAsync()
        {
            return await _context.Profesores.ToListAsync();
        }

        public async Task<Profesor?> ObtenerPorIdAsync(long id)
        {
            return await _context.Profesores.FindAsync(id);
        }
        public async Task<Profesor> AgregarAsync(Profesor profesor)
        {
            profesor.Rol = "Profesor";
            _context.Profesores.Add(profesor);
            await _context.SaveChangesAsync();
            return profesor;
        }
        public async Task<Profesor?> ActualizarAsync(Profesor profesor)
        {
            var existe = await _context.Profesores.FindAsync(profesor.Id);
            if (existe == null)
            {
                return null;
            }

            // Atributos heredados de Usuario
            existe.Nombre = profesor.Nombre;
            existe.Apellido = profesor.Apellido;
            existe.Dni = profesor.Dni;
            existe.NombreUsuario = profesor.NombreUsuario;
            existe.PasswordHash = profesor.PasswordHash;
            existe.Rol = profesor.Rol;
            existe.Sexo = profesor.Sexo;
            existe.QrToken = profesor.QrToken;
            existe.TokenRecuperacion = profesor.TokenRecuperacion;
            existe.TokenExpira = profesor.TokenExpira;
            existe.FechaNacimiento = profesor.FechaNacimiento;
            existe.FotoDePerfil = profesor.FotoDePerfil;
            existe.Estado = profesor.Estado;
            existe.Email = profesor.Email;
            existe.CuotaPagadaHasta = profesor.CuotaPagadaHasta;
            existe.Telefono = profesor.Telefono;
            existe.EsActivado = profesor.EsActivado;

            // Atributos propios de Profesor
            existe.Matricula = profesor.Matricula;
            existe.Sueldo = profesor.Sueldo;


            await _context.SaveChangesAsync();
            return existe;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor == null)
            {
                return false;
            }
            _context.Profesores.Remove(profesor);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<Profesor>> ObtenerPorGimnasioAsync(long gimnasioId)
        {
            return await _context.Profesores
     .Include(p => p.Gimnasio)
     .Where(p => p.GimnasioId == gimnasioId)
     .ToListAsync();
        }

    }
}

