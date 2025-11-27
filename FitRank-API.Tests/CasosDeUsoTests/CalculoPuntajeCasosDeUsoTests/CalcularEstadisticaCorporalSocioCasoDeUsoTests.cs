using FitRank_API.Application.DTOs.CalcularPuntajeDTOs;
using FitRank_API.Application.UseCases;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
{
    public class CalcularEstadisticaCorporalSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly CalcularEstadisticaCorporalSocioCasoDeUso _casoDeUso;

        public CalcularEstadisticaCorporalSocioCasoDeUsoTests()
        {
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _casoDeUso = new CalcularEstadisticaCorporalSocioCasoDeUso(_mockSocioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCalcularIMCCorrectamente_ConPesoNormalYAlturaValida()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Altura = 1.75, // metros
                MedidasCorporales = new List<MedidaCorporal>
                {
                    new MedidaCorporal
                    {
                        Id = 1,
                        PesoKg = 70,
                        Fecha = DateTime.UtcNow
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Imc.Should().BeApproximately(22.86, 0.01); // 70 / (1.75^2)
            resultado.ClasificacionImc.Should().Be("Normal");
            resultado.Peso.Should().Be(70);
            resultado.Altura.Should().Be(1.75);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNull_CuandoSocioNoExiste()
        {
            // Arrange
            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(999))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNull_CuandoSocioNoTieneMedidas()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Altura = 1.75,
                MedidasCorporales = new List<MedidaCorporal>()
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeUsarUltimaMedida_CuandoHayVariasMedidas()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Altura = 1.75,
                MedidasCorporales = new List<MedidaCorporal>
                {
                    new MedidaCorporal
                    {
                        Id = 1,
                        PesoKg = 80,
                        Fecha = DateTime.UtcNow.AddDays(-10)
                    },
                    new MedidaCorporal
                    {
                        Id = 2,
                        PesoKg = 75,
                        Fecha = DateTime.UtcNow.AddDays(-5)
                    },
                    new MedidaCorporal
                    {
                        Id = 3,
                        PesoKg = 70,
                        Fecha = DateTime.UtcNow
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Peso.Should().Be(70);
        }

        [Theory]
        [InlineData(50, 1.75, 16.33, "Bajo peso")] // IMC < 18.5
        [InlineData(70, 1.75, 22.86, "Normal")] // 18.5 <= IMC < 25
        [InlineData(85, 1.75, 27.76, "Sobrepeso")] // 25 <= IMC < 30
        [InlineData(100, 1.75, 32.65, "Obesidad")] // IMC >= 30
        public async Task Ejecutar_DebeClasificarIMCCorrectamente(double peso, double altura, double imcEsperado, string clasificacionEsperada)
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Altura = altura,
                MedidasCorporales = new List<MedidaCorporal>
                {
                    new MedidaCorporal
                    {
                        Id = 1,
                        PesoKg = peso,
                        Fecha = DateTime.UtcNow
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Imc.Should().BeApproximately(imcEsperado, 0.01);
            resultado.ClasificacionImc.Should().Be(clasificacionEsperada);
        }

        [Fact]
        public async Task Ejecutar_DebeRedondearIMCA2Decimales()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Altura = 1.73,
                MedidasCorporales = new List<MedidaCorporal>
                {
                    new MedidaCorporal
                    {
                        Id = 1,
                        PesoKg = 68.5,
                        Fecha = DateTime.UtcNow
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Imc.Should().Be(Math.Round(68.5 / Math.Pow(1.73, 2), 2));
        }

        [Fact]
        public async Task Ejecutar_DebeIncluirFechaDeMedicion()
        {
            // Arrange
            var fechaMedicion = DateTime.UtcNow.AddDays(-2);
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Altura = 1.75,
                MedidasCorporales = new List<MedidaCorporal>
                {
                    new MedidaCorporal
                    {
                        Id = 1,
                        PesoKg = 70,
                        Fecha = fechaMedicion
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConMedidasAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.FechaMedicion.Should().Be(fechaMedicion);
        }
    }
}
