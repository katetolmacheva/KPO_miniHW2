using Microsoft.AspNetCore.Mvc;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Domain.ValueObjects;
using ZooWebApp.Presentation.Models;

namespace ZooWebApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnclosuresController : ControllerBase
{
    private readonly IEnclosureRepository _enclosureRepository;

    public EnclosuresController(IEnclosureRepository enclosureRepository)
    {
        _enclosureRepository = enclosureRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var enclosures = await _enclosureRepository.GetAllAsync();
        return Ok(enclosures);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var enclosure = await _enclosureRepository.GetByIdAsync(id);
        return enclosure != null ? Ok(enclosure) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnclosureDto createDto)
    {
        try
        {
            var enclosure = new Enclosure
            {
                Id = 0,
                Name = createDto.Name,
                Type = createDto.Type,
                Size = createDto.Size,
                MaxCapacity = createDto.MaxCapacity,
                SpeciesType = createDto.SpeciesType,
                CurrentOccupancy = 0
            };
            
            await _enclosureRepository.AddAsync(enclosure);
            
            var enclosures = await _enclosureRepository.GetAllAsync();
            var createdEnclosure = enclosures.LastOrDefault();
            
            if (createdEnclosure == null)
                return StatusCode(500, "Failed to retrieve created enclosure");
                
            return CreatedAtAction(nameof(GetById), new { id = createdEnclosure.Id }, createdEnclosure);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEnclosureDto updateDto)
    {
        try
        {
            var existingEnclosure = await _enclosureRepository.GetByIdAsync(id);
            if (existingEnclosure == null)
                return NotFound();
            
            var updatedEnclosure = new Enclosure
            {
                Id = id,
                Name = updateDto.Name,
                Type = updateDto.Type,
                Size = updateDto.Size,
                CurrentOccupancy = updateDto.CurrentOccupancy,
                MaxCapacity = updateDto.MaxCapacity,
                SpeciesType = updateDto.SpeciesType
            };
            
            await _enclosureRepository.UpdateAsync(updatedEnclosure);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var enclosure = await _enclosureRepository.GetByIdAsync(id);
            if (enclosure == null)
                return NotFound();
                
            await _enclosureRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
