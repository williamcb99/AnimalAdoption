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
            Age = animal.Age,
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
}