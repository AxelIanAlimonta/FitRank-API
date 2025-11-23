
using System.Linq.Expressions;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositorios
{
    public class UsuarioRepositorioImpl : IUsuarioRepositorio
    {
        private readonly FitRankDbContext _context;

        public UsuarioRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistePorEmailAsync(string email)
            => await _context.Usuarios.AnyAsync(u => u.Email == email);

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
            => await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<Usuario?> ObtenerPorIdAsync(long id)
            => await _context.Usuarios.FindAsync(id);

        public async Task<Usuario?> ObtenerPorTokenActivacionAsync(string token)
            => await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.TokenRecuperacion == token && u.TokenExpira > DateTime.UtcNow && !u.EsActivado);




        public async Task<Usuario> AgregarAsync(Usuario usuario)
        {
            if (usuario is Socio socio)
                _context.Socios.Add(socio);
            else
                _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();
            return usuario;
        }

        // 🔹 Obtener por condición, incluyendo tipos derivados
        public async Task<Usuario?> ObtenerPorCondicionAsync(Expression<Func<Usuario, bool>> predicate)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(predicate);
        }


        public async Task<Usuario?> ActualizarAsync(Usuario usuario)
        {
            // Trae la entidad existente (trackeada por EF)
            var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);

            if (existente == null)
                return null;

            // ⚡ Actualizá manualmente los campos importantes
            existente.Nombre = usuario.Nombre;
            existente.Apellido = usuario.Apellido;
            existente.Dni = usuario.Dni;
            existente.NombreUsuario = usuario.NombreUsuario;

            // 🔐 Aseguramos que la nueva contraseña se guarde correctamente
            if (!string.IsNullOrEmpty(usuario.PasswordHash) && usuario.PasswordHash != existente.PasswordHash)
            {
                existente.PasswordHash = usuario.PasswordHash;
            }

            existente.Rol = usuario.Rol;
            existente.Sexo = usuario.Sexo;
            existente.QrToken = usuario.QrToken;

            // ⚡ Token y activación
            existente.TokenRecuperacion = usuario.TokenRecuperacion;
            existente.TokenExpira = usuario.TokenExpira;
            existente.EsActivado = usuario.EsActivado;

            existente.FechaNacimiento = usuario.FechaNacimiento;
            existente.FotoDePerfil = usuario.FotoDePerfil;
            existente.Estado = usuario.Estado;
            existente.Email = usuario.Email;
            existente.CuotaPagadaHasta = usuario.CuotaPagadaHasta;
            existente.Telefono = usuario.Telefono;

            // 🚀 Guardamos todo
            await _context.SaveChangesAsync();

            Console.WriteLine($"[ActualizarAsync] Usuario {existente.Email} actualizado correctamente ✅");

            return existente;
        }



        public async Task EliminarAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
            => await _context.Usuarios.ToListAsync();

        public async Task<Socio?> ObtenerSocioConGimnasioPorIdAsync(long id)
        {
            return await _context.Socios
                .Include(s => s.Gimnasio)
                .FirstOrDefaultAsync(s => s.Id == id);
        }


        public async Task<List<Socio>> ObtenerSociosActivosAsync()
        {

            return await _context.Socios
    .Where(s => s.Estado == "Activo" &&
                s.CuotaPagadaHasta >= DateTime.UtcNow.Date)
    .Include(s => s.Gimnasio)
    .ToListAsync();
        }

    }
}
