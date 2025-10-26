
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

        public async  Task<Usuario> AgregarAsync(Usuario usuario)
        {
            var resultado = await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> ActualizarAsync(Usuario usuario)
        {
            var existenteUsuario = await _context.Usuarios.FindAsync(usuario.Id);
            if (existenteUsuario == null) return null;
            existenteUsuario.Nombre = usuario.Nombre;
            existenteUsuario.Apellido = usuario.Apellido;
            existenteUsuario.Dni = usuario.Dni;
            existenteUsuario.NombreUsuario = usuario.NombreUsuario;
            existenteUsuario.PasswordHash = usuario.PasswordHash;
            existenteUsuario.Rol = usuario.Rol;
            existenteUsuario.Sexo = usuario.Sexo;
            existenteUsuario.QrToken = usuario.QrToken;
            existenteUsuario.TokenRecuperacion = usuario.TokenRecuperacion;
            existenteUsuario.TokenExpira = usuario.TokenExpira;
            existenteUsuario.FechaNacimiento = usuario.FechaNacimiento;
            existenteUsuario.FotoDePerfil = usuario.FotoDePerfil;
            existenteUsuario.Estado = usuario.Estado;
            existenteUsuario.Email = usuario.Email;
            existenteUsuario.CuotaPagadaHasta = usuario.CuotaPagadaHasta;
            existenteUsuario.Telefono = usuario.Telefono;
            existenteUsuario.EsActivado = usuario.EsActivado;
             _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return existenteUsuario;
        }

        public async Task EliminarAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
            => await _context.Usuarios.ToListAsync();


        public async Task<Usuario?> ObtenerPorCondicionAsync(Expression<Func<Usuario, bool>> predicado)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(predicado);
        }
    }
}
