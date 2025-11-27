using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class ActualizarSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ActualizarSocioCasoDeUso _casoDeUso;

        public ActualizarSocioCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<ISocioRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ActualizarSocioCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaActualizarSocioCorrectamente()
        {
            // Arrange
            var socioDTO = new SocioDTO
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                NombreUsuario = "juanp",
                Altura = 1.75,
                Peso = 75.0,
                Nivel = "Intermedio",
                ParticipaEnRanking = true,
                GimnasioId = 1
            };

            var socioEntidad = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                Email = "juan@test.com",
                NombreUsuario = "juanp",
                Altura = 1.75,
                Peso = 75.0,
                Nivel = "Intermedio",
                ParticipaEnRanking = true,
                GimnasioId = 1
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Socio>()))
                .ReturnsAsync(socioEntidad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioDTO);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Nombre.Should().Be("Juan");
            resultado.Apellido.Should().Be("Pérez");
            resultado.NombreUsuario.Should().Be("juanp");
            _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Socio>()), Times.Once);
        }
    }
}
