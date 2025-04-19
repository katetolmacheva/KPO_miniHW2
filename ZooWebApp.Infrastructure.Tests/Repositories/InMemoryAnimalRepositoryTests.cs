using Xunit;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Infrastructure.Repositories;

namespace ZooWebApp.Infrastructure.Tests.Repositories;

public class InMemoryAnimalRepositoryTests
{
    [Fact]
    public async Task AddAsync_SetsIdAndAddsToCollection()
    {
        var repository = new InMemoryAnimalRepository();
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );

        await repository.AddAsync(animal);
        var allAnimals = await repository.GetAllAsync();
        var retrievedAnimal = await repository.GetByIdAsync(1);

        Assert.Single(allAnimals);
        Assert.NotNull(retrievedAnimal);
        Assert.Equal(1, retrievedAnimal.Id);
        Assert.Equal("Leo", retrievedAnimal.Name);
    }

    [Fact]
    public async Task AddAsync_MultipleCalls_AssignsUniqueIds()
    {
        var repository = new InMemoryAnimalRepository();
        var animal1 = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );

        var animal2 = new Animal(
            Species.Tiger,
            "Tony",
            new DateTime(2019, 5, 15),
            Gender.Male,
            FoodType.Meat
        );

        await repository.AddAsync(animal1);
        await repository.AddAsync(animal2);
        var allAnimals = await repository.GetAllAsync();

        Assert.Equal(2, allAnimals.Count());
        Assert.Contains(allAnimals, a => a.Id == 1);
        Assert.Contains(allAnimals, a => a.Id == 2);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenAnimalDoesNotExist()
    {
        var repository = new InMemoryAnimalRepository();

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingAnimal()
    {
        var repository = new InMemoryAnimalRepository();
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        await repository.AddAsync(animal);

        var storedAnimal = await repository.GetByIdAsync(1);
        
        var updatedAnimal = new Animal(
            Species.Lion,
            "Leo Updated",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        typeof(ZooWebApp.Domain.Common.EntityBase).GetProperty("Id")?.SetValue(updatedAnimal, 1);

        await repository.UpdateAsync(updatedAnimal);
        var retrievedAnimal = await repository.GetByIdAsync(1);

        Assert.NotNull(retrievedAnimal);
        Assert.Equal("Leo Updated", retrievedAnimal.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesAnimal()
    {
        var repository = new InMemoryAnimalRepository();
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        await repository.AddAsync(animal);

        await repository.DeleteAsync(1);
        var allAnimals = await repository.GetAllAsync();
        var retrievedAnimal = await repository.GetByIdAsync(1);

        Assert.Empty(allAnimals);
        Assert.Null(retrievedAnimal);
    }
    
    [Fact]
    public async Task GetAllAsync_ReturnsEmptyCollection_WhenNoAnimalsExist()
    {
        var repository = new InMemoryAnimalRepository();
        
        var result = await repository.GetAllAsync();
        
        Assert.Empty(result);
        Assert.IsAssignableFrom<IEnumerable<Animal>>(result);
    }
    
    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenAnimalDoesNotExist()
    {
        var repository = new InMemoryAnimalRepository();
        var nonExistentAnimal = new Animal(
            Species.Lion,
            "Phantom",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        typeof(ZooWebApp.Domain.Common.EntityBase).GetProperty("Id")?.SetValue(nonExistentAnimal, 999);
        
        await repository.UpdateAsync(nonExistentAnimal);
        var result = await repository.GetByIdAsync(999);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenAnimalDoesNotExist()
    {
        var repository = new InMemoryAnimalRepository();
        
        await repository.DeleteAsync(999);
        
    }
}
