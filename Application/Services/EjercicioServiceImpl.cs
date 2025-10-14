using AutoMapper;
using FitRank_API.Application.DTOs.Ejercicionamespace;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Services;

public class EjercicioServiceImpl : IEjercicioService
{
    private readonly IEjercicioRepositorio _ejercicioRepository;
    private readonly IMapper _mapper;

    public EjercicioServiceImpl(IEjercicioRepositorio ejercicioRepository, IMapper mapper)
    {
        _ejercicioRepository = ejercicioRepository;
        _mapper = mapper;
    }

    public Task<EjercicioDTO?> CreateAsync(CrearEjercicioDTO ejercicioDto)
    {
        var ejercicio = _mapper.Map<Ejercicio>(ejercicioDto);

        return _ejercicioRepository.AddAsync(ejercicio)
            .ContinueWith(task => task.Result == null ? null : _mapper.Map<EjercicioDTO>(task.Result));
    }

    //getall
    public async Task<List<EjercicioDTO>> GetAllAsync()
    {
        var ejercicios = await _ejercicioRepository.GetAllAsync();
        return _mapper.Map<List<EjercicioDTO>>(ejercicios);
    }

    public async Task<EjercicioDTO> GetByIdAsync(long id)
    {
        var ejercicio = await _ejercicioRepository.GetByIdAsync(id);
        if (ejercicio == null)
        {
            throw new KeyNotFoundException("Ejercicio not found");
        }
        return _mapper.Map<EjercicioDTO>(ejercicio);
    }

    public async Task<EjercicioDTO> UpdateAsync(long id, EjercicioDTO ejercicioDto)
    {
        var existingEjercicio = await _ejercicioRepository.GetByIdAsync(id);
        if (existingEjercicio == null)
        {
            throw new KeyNotFoundException("Ejercicio not found");
        }
        var ejercicio = _mapper.Map<Ejercicio>(ejercicioDto);
        ejercicio.Id = id; // Asegurarse de que el ID se mantenga igual
        await _ejercicioRepository.UpdateAsync(ejercicio);
        return _mapper.Map<EjercicioDTO>(ejercicio);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var existingEjercicio = await _ejercicioRepository.GetByIdAsync(id);
        if (existingEjercicio == null)
        {
            return false;
        }
        await _ejercicioRepository.DeleteAsync(id);
        return true;
    }


}
