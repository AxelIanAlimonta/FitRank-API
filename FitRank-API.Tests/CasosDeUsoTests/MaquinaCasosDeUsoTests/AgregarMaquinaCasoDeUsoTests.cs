using AutoMapper;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CasosDeUsoTests.MaquinaCasosDeUsoTests
{
    public class AgregarMaquinaCasoDeUsoTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;
        private readonly QrHelper _qrHelperReal;

        public AgregarMaquinaCasoDeUsoTests()
        {
            // CONFIG FAKE PARA QRHELPER
            var configFake = new Dictionary<string, string>
            {
                { "BaseUrls:Frontend", "http://fake-frontend.com" },
                { "QrSecret", "12345678901234567890123456789012" },
                { "Jwt:Issuer", "FakeIssuer" },
                { "Jwt:Audience", "FakeAudience" },
                { "Jwt:Key", "ClaveJwtParaTests12345678901234567890" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configFake)
                .Build();

            _qrHelperReal = new QrHelper(configuration);

            // CONFIG AUTOMAPPER REAL
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AgregarMaquinaDTO, Maquina>();
                cfg.CreateMap<Maquina, ObtenerMaquinaDTO>();
            });

            _mapper = mapperConfig.CreateMapper();

            _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();
        }

        [Fact]
        public async Task AgregarMaquina_CuandoLosDatosSonValidos_RetornaMaquinaDTO()
        {
            // Arrange
            long gimnasioId = 1;

            var agregarMaquinaDTO = new AgregarMaquinaDTO
            {
                Nombre = "Maquina Nueva",
                UrlImagen = "http://imagen.nueva"
            };

            var maquinaAgregada = new Maquina
            {
                Id = 10,
                GimnasioId = gimnasioId,
                Nombre = agregarMaquinaDTO.Nombre,
                UrlImagen = agregarMaquinaDTO.UrlImagen,
                Qr = "PENDIENTE"
            };

            // Mock agregar
            _maquinaRepositorioMock
                .Setup(r => r.AgregarMaquina(It.IsAny<Maquina>()))
                .ReturnsAsync(maquinaAgregada);

            // Mock actualizar
            _maquinaRepositorioMock
       .Setup(r => r.AgregarMaquina(It.IsAny<Maquina>()))
       .ReturnsAsync(maquinaAgregada);


            var casoDeUso = new AgregarMaquinaCasoDeUso(
                _maquinaRepositorioMock.Object,
                _mapper,
                _qrHelperReal
            );

            // Act
            var resultado = await casoDeUso.Ejecutar(agregarMaquinaDTO, gimnasioId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(maquinaAgregada.Id, resultado.Id);
            Assert.Equal(gimnasioId, resultado.GimnasioId);
            Assert.Equal(agregarMaquinaDTO.Nombre, resultado.Nombre);
            Assert.Equal(agregarMaquinaDTO.UrlImagen, resultado.UrlImagen);

            Assert.NotNull(resultado.Qr);

        }
    }
}
