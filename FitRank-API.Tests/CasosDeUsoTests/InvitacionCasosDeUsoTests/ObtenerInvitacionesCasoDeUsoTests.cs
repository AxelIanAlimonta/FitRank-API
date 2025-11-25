using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class ObtenerInvitacionesCasoDeUsoTests
    {
        private readonly Mock<IInvitacionRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerInvitacionesCasoDeUso _casoDeUso;

        public ObtenerInvitacionesCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IInvitacionRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvitacionProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerInvitacionesCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodasLasInvitacionesDelGimnasio()
        {
            // Arrange
            var gimnasioId = 1;
            var invitaciones = new List<Invitacion>
            {
                new Invitacion
                {
                    Id = 1,
                    GimnasioId = gimnasioId,
                    Email = "test1@test.com",
                    MetodoPago = "Efectivo",
                    Estado = "Pagado",
                    CreadaEn = DateTime.Now.AddDays(-5),
                    ExpiraEn = DateTime.Now.AddDays(19),
                    CuotaPagadaHasta = DateTime.Now.AddMonths(1)
                },
                new Invitacion
                {
                    Id = 2,
                    GimnasioId = gimnasioId,
                    Email = "test2@test.com",
                    MetodoPago = "MercadoPago",
                    Estado = "Pendiente",
                    CreadaEn = DateTime.Now.AddDays(-1),
                    ExpiraEn = DateTime.Now.AddDays(23)
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync(gimnasioId))
                .ReturnsAsync(invitaciones);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().Email.Should().Be("test1@test.com");
            resultado.Last().Email.Should().Be("test2@test.com");
            _mockRepositorio.Verify(r => r.ObtenerTodasAsync(gimnasioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayInvitaciones()
        {
            // Arrange
            var gimnasioId = 1;
            var invitacionesVacias = new List<Invitacion>();

            _mockRepositorio.Setup(r => r.ObtenerTodasAsync(gimnasioId))
                .ReturnsAsync(invitacionesVacias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
           
        }
    }
}
