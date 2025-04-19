using System;
using System.Linq;
using Xunit;
using ZooWebApp.Domain.Entities;
using ZooWebApp.Domain.Enums;
using ZooWebApp.Domain.Events;
using ZooWebApp.Domain.ValueObjects;

namespace ZooWebApp.Domain.Tests.Entities;

public class AnimalTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var species = Species.Lion;
        var name = "Leo";
        var birthDate = new DateTime(2020, 1, 1);
        var gender = Gender.Male;
        var favoriteFood = FoodType.Meat;

        var animal = new Animal(species, name, birthDate, gender, favoriteFood);

        Assert.Equal(species, animal.Species);
        Assert.Equal(name, animal.Name);
        Assert.Equal(birthDate, animal.BirthDate);
        Assert.Equal(gender, animal.Gender);
        Assert.Equal(favoriteFood, animal.FavoriteFood);
        Assert.Equal(HealthStatus.Healthy, animal.HealthStatus);
        Assert.Null(animal.Enclosure);
        Assert.Empty(animal.FeedingSchedules);
    }

    [Fact]
    public void Feed_WhenHealthy_ShouldAddScheduleAndCreateEvent()
    {
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        var feedingTime = new DateTime(2023, 5, 15, 10, 0, 0);

        animal.Feed(feedingTime);

        Assert.Single(animal.FeedingSchedules);
        Assert.Equal(feedingTime.TimeOfDay, animal.FeedingSchedules.First().Time);
        Assert.Single(animal.FeedingSchedules.First().FoodItems);
        Assert.Equal(FoodType.Meat, animal.FeedingSchedules.First().FoodItems.First());

        Assert.Single(animal.DomainEvents);
        var feedingEvent = animal.DomainEvents.First() as FeedingTimeEvent;
        Assert.NotNull(feedingEvent);
        Assert.Equal(animal.Id, feedingEvent.AnimalId);
        Assert.Equal("Leo", feedingEvent.AnimalName);
        Assert.Equal(feedingTime.TimeOfDay, feedingEvent.FeedingTime);
    }

    [Fact]
    public void Feed_WhenSick_ShouldThrowException()
    {
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        animal.Treat();
        var feedingTime = new DateTime(2023, 5, 15, 10, 0, 0);
        
        typeof(Animal).GetProperty(nameof(Animal.HealthStatus))
            .SetValue(animal, HealthStatus.Sick);

        var exception = Assert.Throws<InvalidOperationException>(() => animal.Feed(feedingTime));
        Assert.Equal("Cannot feed a sick animal", exception.Message);
    }

    [Fact]
    public void Treat_ShouldSetHealthStatusToHealthy()
    {
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        
        typeof(Animal).GetProperty(nameof(Animal.HealthStatus))
            .SetValue(animal, HealthStatus.Sick);

        animal.Treat();

        Assert.Equal(HealthStatus.Healthy, animal.HealthStatus);
    }

    [Fact]
    public void MoveToEnclosure_ShouldSetEnclosureAndCreateEvent()
    {
        var animal = new Animal(
            Species.Lion,
            "Leo",
            new DateTime(2020, 1, 1),
            Gender.Male,
            FoodType.Meat
        );
        var enclosure = new Enclosure
        {
            Name = "Lion's Den",
            Type = EnclosureType.Cage,
            Size = 100.0,
            MaxCapacity = 5,
            CurrentOccupancy = 2,
            SpeciesType = Species.Lion
        };

        animal.MoveToEnclosure(enclosure);

        Assert.Equal(enclosure, animal.Enclosure);
        
        Assert.Single(animal.DomainEvents);
        var transferEvent = animal.DomainEvents.First() as AnimalTransferredEvent;
        Assert.NotNull(transferEvent);
        Assert.Equal(animal.Id, transferEvent.AnimalId);
        Assert.Equal("None", transferEvent.PreviousEnclosure);
        Assert.Equal("Lion's Den", transferEvent.NewEnclosure);
    }
}
