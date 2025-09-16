using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<List<AnimalSummaryDto>>> GetAllAnimals()
    {
        var animals = await _animalRepository.GetAnimalsAsync();
        if (animals == null || !animals.Any())
        {
            return Problem("Animal repository returned null or empty list.");
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
    public async Task<ActionResult<AnimalDetailedDto>> GetAnimalById(int id)
    {
        var animal = await _animalRepository.GetAnimalByIdAsync(id);
        if (animal == null)
        {
            return NotFound();
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
}