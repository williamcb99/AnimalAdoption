public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimalType Type { get; set; }
    public DateOnly Birthdate { get; set; }
    public DateOnly ArrivalDate { get; set; }
    public string Breed { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAdopted { get; set; }
}