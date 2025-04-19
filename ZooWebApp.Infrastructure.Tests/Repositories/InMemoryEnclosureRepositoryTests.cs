using Xunit;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.ValueObjects;
using ZooWebApp.Infrastructure.Repositories;

namespace ZooWebApp.Infrastructure.Tests.Repositories;

public class InMemoryEnclosureRepositoryTests
{
    [Fact]
    public async Task AddAsync_SetsIdAndAddsToCollection()
    {
        var repository = new InMemoryEnclosureRepository();
        var enclosure = new Enclosure
        {
            Id = 0,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };

        await repository.AddAsync(enclosure);
        var allEnclosures = await repository.GetAllAsync();
        var retrievedEnclosure = await repository.GetByIdAsync(1);

        Assert.Single(allEnclosures);
        Assert.NotNull(retrievedEnclosure);
        Assert.Equal(1, retrievedEnclosure.Id);
        Assert.Equal("Lion Den", retrievedEnclosure.Name);
    }

    [Fact]
    public async Task AddAsync_MultipleCalls_AssignsUniqueIds()
    {
        var repository = new InMemoryEnclosureRepository();
        var enclosure1 = new Enclosure
        {
            Id = 0,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };

        var enclosure2 = new Enclosure
        {
            Id = 0,
            Name = "Tiger Cage",
            Type = EnclosureType.Cage,
            Size = 80.0,
            MaxCapacity = 3,
            CurrentOccupancy = 0,
            SpeciesType = Species.Tiger
        };

        await repository.AddAsync(enclosure1);
        await repository.AddAsync(enclosure2);
        var allEnclosures = await repository.GetAllAsync();

        Assert.Equal(2, allEnclosures.Count());
        Assert.Contains(allEnclosures, e => e.Id == 1);
        Assert.Contains(allEnclosures, e => e.Id == 2);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEnclosureDoesNotExist()
    {
        var repository = new InMemoryEnclosureRepository();

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingEnclosure()
    {
        var repository = new InMemoryEnclosureRepository();
        var enclosure = new Enclosure
        {
            Id = 0,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };
        await repository.AddAsync(enclosure);

        var updatedEnclosure = new Enclosure
        {
            Id = 1,
            Name = "Lion Den Updated",
            Type = EnclosureType.OpenAir,
            Size = 120.5,
            MaxCapacity = 8,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };
        await repository.UpdateAsync(updatedEnclosure);
        var retrievedEnclosure = await repository.GetByIdAsync(1);

        Assert.NotNull(retrievedEnclosure);
        Assert.Equal("Lion Den Updated", retrievedEnclosure.Name);
        Assert.Equal(120.5, retrievedEnclosure.Size);
        Assert.Equal(8, retrievedEnclosure.MaxCapacity);
        Assert.Equal(2, retrievedEnclosure.CurrentOccupancy);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEnclosure()
    {
        var repository = new InMemoryEnclosureRepository();
        var enclosure = new Enclosure
        {
            Id = 0,
            Name = "Lion Den",
            Type = EnclosureType.OpenAir,
            Size = 100.5,
            MaxCapacity = 5,
            CurrentOccupancy = 0,
            SpeciesType = Species.Lion
        };
        await repository.AddAsync(enclosure);

        await repository.DeleteAsync(1);
        var allEnclosures = await repository.GetAllAsync();
        var retrievedEnclosure = await repository.GetByIdAsync(1);

        Assert.Empty(allEnclosures);
        Assert.Null(retrievedEnclosure);
    }
}
