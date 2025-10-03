using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace FitRank_API.Infrastructure.Repositories;

public class PersonaRepositoryImpl : IPersonaRepository
{
    private readonly FitRankDbContext _context;
    public PersonaRepositoryImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Domain.Entities.Persona persona)
    {
        await _context.Personas.AddAsync(persona);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var persona = await _context.Personas.FindAsync(id);
        if (persona != null)
        {
            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Domain.Entities.Persona>> GetAllAsync()
    {
        return await _context.Personas.ToListAsync();
    }

    public async Task<Domain.Entities.Persona?> GetByIdAsync(long id)
    {
        return await _context.Personas.FindAsync(id);
    }

    public async Task UpdateAsync(Domain.Entities.Persona persona)
    {
        _context.Personas.Update(persona);
        await _context.SaveChangesAsync();
    }



}
