using AutoMapper;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Presentacion.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FitRank_API.tests.ControllersTests
{
    public class MaquinaControllerTests
    {
        private readonly Mock<ObtenerMaquinasCasoDeUso> _mockObtenerTodos;
        private readonly Mock<AgregarMaquinaCasoDeUso> _mockAgregar;
        private readonly Mock<ActualizarMaquinaCasoDeUso> _mockActualizar;
        private readonly Mock<EliminarMaquinaCasoDeUso> _mockEliminar;
        private readonly Mock<ObtenerMaquinaPorIdCasoDeUso> _mockObtenerPorId;
        private readonly Mock<ObtenerMaquinaDetalleCasoDeUso> _mockDetalle;

        private readonly MaquinaController _controller;

        private const long GIMNASIO_ID = 99;

        public MaquinaControllerTests()
        {
            _mockObtenerTodos = new Mock<ObtenerMaquinasCasoDeUso>();
            _mockAgregar = new Mock<AgregarMaquinaCasoDeUso>();
            _mockActualizar = new Mock<ActualizarMaquinaCasoDeUso>();
            _mockEliminar = new Mock<EliminarMaquinaCasoDeUso>();
            _mockObtenerPorId = new Mock<ObtenerMaquinaPorIdCasoDeUso>();
            _mockDetalle = new Mock<ObtenerMaquinaDetalleCasoDeUso>();

            _controller = new MaquinaController(
                _mockObtenerTodos.Object,
                _mockAgregar.Object,
                _mockActualizar.Object,
                _mockEliminar.Object,
                _mockObtenerPorId.Object,
                _mockDetalle.Object
            );

            // Simular usuario con claim del gimnasio
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.GroupSid, GIMNASIO_ID.ToString())
                    }
                )
            );

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        // ---------------------------------------
        // CREAR
        // ---------------------------------------
    
        [Fact]
        public async Task Crear_RetornaBadRequest_SiDTOEsNulo()
        {
            var result = await _controller.Crear(null);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
            Assert.Equal("El objeto no puede ser nulo.", bad.Value);
        }

        [Fact]
        public async Task Crear_Excepcion_Retorna500()
        {
            var dto = new AgregarMaquinaDTO { Nombre = "Test" };

            _mockAgregar
                .Setup(m => m.Ejecutar(dto, GIMNASIO_ID))
                .ThrowsAsync(new Exception("Error de servidor."));

            var result = await _controller.Crear(dto);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
            Assert.Equal("Error de servidor.", error.Value);
        }

        // ---------------------------------------
        // OBTENER POR ID
        // ---------------------------------------
        [Fact]
        public async Task ObtenerPorId_NoExiste_RetornaNotFound()
        {
            _mockObtenerPorId
                .Setup(m => m.Ejecutar(777))
                .ReturnsAsync((ObtenerMaquinaDTO)null);

            var result = await _controller.ObtenerPorId(777);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ObtenerPorId_Existe_RetornaOk()
        {
            var dto = new ObtenerMaquinaDTO { Id = 1, Nombre = "Test" };

            _mockObtenerPorId
                .Setup(m => m.Ejecutar(1))
                .ReturnsAsync(dto);

            var result = await _controller.ObtenerPorId(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        // ---------------------------------------
        // ACTUALIZAR
        // ---------------------------------------
        [Fact]
        public async Task Actualizar_Ok()
        {
            var dto = new ActualizarMaquinaDTO
            {
                Id = 1,
                Nombre = "Actualizada"
            };

            var actualizado = new ObtenerMaquinaDTO
            {
                Id = 1,
                Nombre = "Actualizada"
            };

            _mockActualizar
                .Setup(m => m.Ejecutar(dto))
                .ReturnsAsync(actualizado);

            var result = await _controller.Actualizar(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, ok.StatusCode);
        }

        [Fact]
        public async Task Actualizar_IdNoCoincide_400()
        {
            var dto = new ActualizarMaquinaDTO { Id = 2, Nombre = "X" };

            var result = await _controller.Actualizar(1, dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, bad.StatusCode);
        }

        [Fact]
        public async Task Actualizar_DTONulo_400()
        {
            var result = await _controller.Actualizar(1, null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Actualizar_NoEncontrado_404()
        {
            var dto = new ActualizarMaquinaDTO { Id = 1 };

            _mockActualizar
                .Setup(m => m.Ejecutar(dto))
                .ReturnsAsync((ObtenerMaquinaDTO)null);

            var result = await _controller.Actualizar(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Actualizar_Excepcion_500()
        {
            var dto = new ActualizarMaquinaDTO { Id = 1 };

            _mockActualizar
                .Setup(m => m.Ejecutar(dto))
                .ThrowsAsync(new Exception("Error de servidor."));

            var result = await _controller.Actualizar(1, dto);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }

        // ---------------------------------------
        // ELIMINAR
        // ---------------------------------------
        [Fact]
        public async Task Eliminar_Ok_NoContent()
        {
            _mockEliminar.Setup(m => m.Ejecutar(1)).ReturnsAsync(true);

            var result = await _controller.Eliminar(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Eliminar_NoExiste_404()
        {
            _mockEliminar.Setup(m => m.Ejecutar(999)).ReturnsAsync(false);

            var result = await _controller.Eliminar(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Eliminar_Excepcion_500()
        {
            _mockEliminar
                .Setup(m => m.Ejecutar(1))
                .ThrowsAsync(new Exception("Error de servidor."));

            var result = await _controller.Eliminar(1);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
        }
    }
}
