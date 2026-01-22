using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalRepository _animalRepository;
    public AnimalController(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    [HttpGet]
    [EndpointDescription("Get a list of all animals available for adoption.")]
    [ProducesResponseType(typeof(List<AnimalSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<AnimalSummaryDto>>> GetAllAnimals()
    {
        var animals = await _animalRepository.GetAnimalsAsync();
        if (animals == null || !animals.Any())
        {
            return Problem(
                title: "Service Unavailable",
                detail: "Animal repository returned null or empty list.",
                statusCode: 503
            );
        }

        var dtos = animals.Select(a => new AnimalSummaryDto
        {
            Id = a.Id,
            Name = a.Name,
            Type = a.Type,
            Links = new List<LinkDto>
            {
                Url.BuildLink("GetAnimalById", new { id = a.Id }, "self", "GET"),
                Url.BuildLink("DeleteAnimal", new { id = a.Id }, "delete", "DELETE")
            }
        }).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}", Name = "GetAnimalById")]
    [EndpointDescription("Get detailed information about a specific animal by its ID.")]
    [ProducesResponseType(typeof(AnimalDetailedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AnimalDetailedDto>> GetAnimalById(int id)
    {
        var animal = await _animalRepository.GetAnimalByIdAsync(id);
        if (animal == null)
        {
            return Problem(
                title: "Animal Not Found",
                detail: $"No animal found with ID {id}.",
                statusCode: 404
            );
        }

        var dto = new AnimalDetailedDto
        {
            Id = animal.Id,
            Name = animal.Name,
            Type = animal.Type,
            Birthdate = animal.Birthdate,
            ArrivalDate = animal.ArrivalDate,
            Breed = animal.Breed,
            Description = animal.Description,
            IsAdopted = animal.IsAdopted,
            Links = new List<LinkDto>
            {
                Url.BuildLink("GetAnimalById", new { id = animal.Id }, "self", "GET"),
                Url.BuildLink("DeleteAnimal", new { id = animal.Id }, "delete", "DELETE"),
                Url.BuildLink("UpdateAnimal", new { id = animal.Id }, "update", "PUT")
            }
        };
        return Ok(dto);
    }

    [HttpDelete("{id}", Name = "DeleteAnimal")]
    [EndpointDescription("Delete an animal from the adoption list by its ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAnimal(int id)
    {
        try
        {
            await _animalRepository.RemoveAnimalAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return Problem(
                title: "Animal Not Found",
                detail: ex.Message,
                statusCode: 404
            );
        }
    }

    [HttpPost(Name = "AddAnimal")]
    [EndpointDescription("Add a new animal to the adoption list.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddAnimal([FromBody] AddAnimalDto animal)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (animal.Birthdate > DateOnly.FromDateTime(DateTime.Today))
            return Problem(
                title: "Invalid Birthdate",
                detail: "Birthdate cannot be in the future.",
                statusCode: 400
            );
        if (Enum.GetValues<AnimalType>().Cast<AnimalType>().All(t => t != animal.AnimalType))
            return Problem(
                title: "Invalid Animal Type",
                detail: $"Animal type '{animal.AnimalType}' is not recognized.",
                statusCode: 400
            );
        var newAnimal = await _animalRepository.AddAnimalAsync(new Animal {
            Name = animal.Name,
            Type = animal.AnimalType,
            Birthdate = animal.Birthdate,
            ArrivalDate = DateOnly.FromDateTime(DateTime.Today),
            Breed = animal.Breed,
            Description = animal.Description,
            IsAdopted = false
        });

        return CreatedAtAction("GetAnimalById", new { id = newAnimal.Id }, new AnimalSummaryDto
        {
            Id = newAnimal.Id,
            Name = newAnimal.Name,
            Type = newAnimal.Type,
            Links = new List<LinkDto>
            {
                Url.BuildLink("GetAnimalById", new { id = newAnimal.Id }, "self", "GET"),
                Url.BuildLink("DeleteAnimal", new { id = newAnimal.Id }, "delete", "DELETE")
            }
        });
    }
}