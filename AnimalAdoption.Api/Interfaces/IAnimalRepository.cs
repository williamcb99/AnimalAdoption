public interface IAnimalRepository
{
    Task<List<Animal>> GetAnimalsAsync();
    Task<Animal?> GetAnimalByIdAsync(int id);
    Task<Animal> AddAnimalAsync(Animal animal);
    Task RemoveAnimalAsync(int id);
}