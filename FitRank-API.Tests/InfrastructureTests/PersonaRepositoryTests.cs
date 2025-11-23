using System.Threading.Tasks;
using System.Collections.Generic;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class PersonaRepositoryTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AddAsync_DeberiaAgregarPersona()
    {
        var options = CreateInMemoryOptions("AddPersonaDb");
        using var context = new FitRankDbContext(options);
        var repo = new PersonaRepositoryImpl(context);

        var persona = new Persona { };
        await repo.AddAsync(persona);

        var enDb = await context.Personas.FindAsync(persona.Id);
        enDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_DeberiaRetornarTodasLasPersonas()
    {
        var options = CreateInMemoryOptions("GetAllPersonaDb");
        using var context = new FitRankDbContext(options);
        var repo = new PersonaRepositoryImpl(context);

        context.Personas.AddRange(new Persona { }, new Persona { });
        await context.SaveChangesAsync();

        var resultado = await repo.GetAllAsync();
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_DeberiaRetornarPersonaCorrecta()
    {
        var options = CreateInMemoryOptions("GetByIdPersonaDb");
        using var context = new FitRankDbContext(options);
        var repo = new PersonaRepositoryImpl(context);

        var persona = new Persona { };
        context.Personas.Add(persona);
        await context.SaveChangesAsync();

        var resultado = await repo.GetByIdAsync(persona.Id);
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(persona.Id);
    }

    [Fact]
    public async Task UpdateAsync_DeberiaActualizarPersona()
    {
        var options = CreateInMemoryOptions("UpdatePersonaDb");
        using var context = new FitRankDbContext(options);
        var repo = new PersonaRepositoryImpl(context);

        var persona = new Persona { };
        context.Personas.Add(persona);
        await context.SaveChangesAsync();

        // Aquí podrías modificar propiedades si las tuviera
        await repo.UpdateAsync(persona);

        var actualizado = await context.Personas.FindAsync(persona.Id);
        actualizado.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_DeberiaEliminarPersona()
    {
        var options = CreateInMemoryOptions("DeletePersonaDb");
        using var context = new FitRankDbContext(options);
        var repo = new PersonaRepositoryImpl(context);

        var persona = new Persona { };
        context.Personas.Add(persona);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(persona.Id);

        var enDb = await context.Personas.FindAsync(persona.Id);
        enDb.Should().BeNull();
    }
}