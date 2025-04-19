using ZooWebApp.Domain.Enums;
using System;

namespace ZooWebApp.Domain.ValueObjects;

public record Enclosure
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required EnclosureType Type { get; init; }
    public required double Size { get; init; }
    public int CurrentOccupancy { get; init; }
    public int MaxCapacity { get; init; }
    public Species SpeciesType { get; init; }
    
    public virtual bool Equals(Enclosure? other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Id == other.Id &&
               Name == other.Name &&
               Type == other.Type &&
               Size == other.Size &&
               CurrentOccupancy == other.CurrentOccupancy &&
               MaxCapacity == other.MaxCapacity &&
               SpeciesType == other.SpeciesType;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Id, 
            Name, 
            Type, 
            Size.GetHashCode(), 
            CurrentOccupancy, 
            MaxCapacity, 
            SpeciesType
        );
    }
}

public enum EnclosureType
{
    OpenAir,
    Cage,
    Aquarium,
    Terrarium,
    Aviary
}
