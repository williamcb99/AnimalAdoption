using Microsoft.EntityFrameworkCore;

public class AnimalRepository : IAnimalRepository
{
    private readonly AnimalAdoptionDbContext _context;

    public AnimalRepository(AnimalAdoptionDbContext context)
    {
        _context = context;
    }

    public async Task<List<Animal>> GetAnimalsAsync()
    {
        return await _context.Animals.ToListAsync();
    }

    public async Task<Animal?> GetAnimalByIdAsync(int id)
    {
        return await _context.Animals.FindAsync(id);
    }

    public async Task<Animal> AddAnimalAsync(Animal animal)
    {
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();
        return animal;
    }

    public async Task RemoveAnimalAsync(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal != null)
        {
            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();          
        }
    }
}