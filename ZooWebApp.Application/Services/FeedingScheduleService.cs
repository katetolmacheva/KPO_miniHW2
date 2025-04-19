using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.Events;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Application.Services;

public class FeedingScheduleService : IFeedingScheduleService
{
    private readonly IAnimalRepository _animalRepository;

    public FeedingScheduleService(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<IEnumerable<FeedingSchedule>> GetAllSchedulesAsync()
    {
        var animals = await _animalRepository.GetAllAsync();
        return animals.SelectMany(a => a.FeedingSchedules);
    }

    public async Task<IEnumerable<FeedingSchedule>> GetSchedulesByAnimalIdAsync(int animalId)
    {
        var animal = await _animalRepository.GetByIdAsync(animalId);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with ID {animalId} not found");
        }
        
        return animal.FeedingSchedules;
    }

    public async Task AddFeedingScheduleAsync(int animalId, FeedingSchedule schedule)
    {
        var animal = await _animalRepository.GetByIdAsync(animalId);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with ID {animalId} not found");
        }

        var feedingTime = DateTime.Today.Add(schedule.Time);
        animal.Feed(feedingTime);
        
        await _animalRepository.UpdateAsync(animal);
    }

    public async Task MarkFeedingCompleteAsync(int animalId, TimeSpan time)
    {
        var animal = await _animalRepository.GetByIdAsync(animalId);
        if (animal == null)
        {
            throw new KeyNotFoundException($"Animal with ID {animalId} not found");
        }

        var feedingTime = DateTime.Today.Add(time);
        animal.Feed(feedingTime);
        
        await _animalRepository.UpdateAsync(animal);
    }
}
