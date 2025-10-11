using AutoMapper;
using FitRank_API.Application.DTOs.Usuario;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.Services
{
    public class UsuarioServiceImpl : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly FitRankDbContext _context;

        public UsuarioServiceImpl(IUsuarioRepository usuarioRepository,
                                  IMapper mapper,
                                  FitRankDbContext context)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _context = context;
        }



        public async Task<UsuariorRespuestaDto> CrearUsuarioAsync(CrearUsuarioDTO usuarioDto)
        {
            if (usuarioDto == null)
            {
                throw new ArgumentNullException(nameof(usuarioDto));
            }
           if( await BuscarUsuarioPorDni(usuarioDto.dni) != null)
            {
                throw new Exception("El usuario con ese DNI ya existe");
            }
            var usuario = _mapper.Map<Domain.Entities.Usuario>(usuarioDto);
            await _usuarioRepository.AgregarUsuario(usuario);
            
            return _mapper.Map<UsuariorRespuestaDto>(usuario);
        }

        public async Task<UsuarioDTO> GetUsuarioByIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return new UsuarioDTO
                {
                    id = 0,
                    nombre = "No encontrado",
                    email = "Usuario no existe"
                };
            return _mapper.Map<UsuarioDTO>(usuario);
        }

        public async Task<UsuarioDTO> BuscarUsuarioPorDni(int dni)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.dni == dni); 
            if (usuario == null)
                return null;
            return _mapper.Map<UsuarioDTO>(usuario);
        }



    }
}