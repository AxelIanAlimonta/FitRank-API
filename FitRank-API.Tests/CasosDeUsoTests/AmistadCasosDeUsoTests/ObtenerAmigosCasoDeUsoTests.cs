using AutoMapper;
using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AmistadCasosDeUsoTests
{
    public class ObtenerAmigosCasoDeUsoTests
    {
        private readonly Mock<IAmistadRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerAmigosCasoDeUso _casoDeUso;

        public ObtenerAmigosCasoDeUsoTests()
        {
            _mockRepo = new Mock<IAmistadRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerAmigosCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVaciaSiNoTieneAmigos()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorSocioIdAsync(5, EstadoAmistad.Aceptado))
                .ReturnsAsync(new List<Amistad>());
            _mockMapper.Setup(m => m.Map<List<AmigoDTO>>(It.IsAny<List<Socio>>()))
                .Returns(new List<AmigoDTO>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarAmigosCuandoEsSocioId1()
        {
            // Arrange
            var socio1 = new Socio { Id = 5, Nombre = "Socio5" };
            var socio2 = new Socio { Id = 10, Nombre = "Amigo10" };
            var socio3 = new Socio { Id = 15, Nombre = "Amigo15" };

            var amistades = new List<Amistad>
            {
                new Amistad { SocioId1 = 5, SocioId2 = 10, Socio1 = socio1, Socio2 = socio2, Estado = EstadoAmistad.Aceptado },
                new Amistad { SocioId1 = 5, SocioId2 = 15, Socio1 = socio1, Socio2 = socio3, Estado = EstadoAmistad.Aceptado }
            };

            _mockRepo.Setup(r => r.ObtenerPorSocioIdAsync(5, EstadoAmistad.Aceptado))
                .ReturnsAsync(amistades);

            var amigosDTO = new List<AmigoDTO>
            {
                new AmigoDTO { Id = 10, Nombre = "Amigo10" },
                new AmigoDTO { Id = 15, Nombre = "Amigo15" }
            };
            _mockMapper.Setup(m => m.Map<List<AmigoDTO>>(It.IsAny<List<Socio>>()))
                .Returns(amigosDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().Contain(a => a.Id == 10);
            resultado.Should().Contain(a => a.Id == 15);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarAmigosCuandoEsSocioId2()
        {
            // Arrange
            var socio1 = new Socio { Id = 3, Nombre = "Amigo3" };
            var socio2 = new Socio { Id = 10, Nombre = "Socio10" };

            var amistades = new List<Amistad>
            {
                new Amistad { SocioId1 = 3, SocioId2 = 10, Socio1 = socio1, Socio2 = socio2, Estado = EstadoAmistad.Aceptado }
            };

            _mockRepo.Setup(r => r.ObtenerPorSocioIdAsync(10, EstadoAmistad.Aceptado))
                .ReturnsAsync(amistades);

            var amigosDTO = new List<AmigoDTO>
            {
                new AmigoDTO { Id = 3, Nombre = "Amigo3" }
            };
            _mockMapper.Setup(m => m.Map<List<AmigoDTO>>(It.IsAny<List<Socio>>()))
                .Returns(amigosDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(10);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Id.Should().Be(3);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConEstadoAceptado()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerPorSocioIdAsync(7, EstadoAmistad.Aceptado))
                .ReturnsAsync(new List<Amistad>());
            _mockMapper.Setup(m => m.Map<List<AmigoDTO>>(It.IsAny<List<Socio>>()))
                .Returns(new List<AmigoDTO>());

            // Act
            await _casoDeUso.Ejecutar(7);

            // Assert
            _mockRepo.Verify(r => r.ObtenerPorSocioIdAsync(7, EstadoAmistad.Aceptado), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteConMapper()
        {
            // Arrange
            var socio1 = new Socio { Id = 1, Nombre = "Socio1" };
            var socio2 = new Socio { Id = 2, Nombre = "Amigo2" };

            var amistades = new List<Amistad>
            {
                new Amistad { SocioId1 = 1, SocioId2 = 2, Socio1 = socio1, Socio2 = socio2 }
            };

            _mockRepo.Setup(r => r.ObtenerPorSocioIdAsync(1, EstadoAmistad.Aceptado))
                .ReturnsAsync(amistades);

            var amigosDTO = new List<AmigoDTO> { new AmigoDTO { Id = 2 } };
            _mockMapper.Setup(m => m.Map<List<AmigoDTO>>(It.IsAny<List<Socio>>()))
                .Returns(amigosDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            _mockMapper.Verify(m => m.Map<List<AmigoDTO>>(It.IsAny<List<Socio>>()), Times.Once);
            resultado.Should().NotBeNull();
        }
    }
}
