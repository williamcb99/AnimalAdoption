using System.ComponentModel.DataAnnotations;

public class AddAnimalDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public AnimalType AnimalType { get; set; }
    [Required]
    public DateOnly Birthdate { get; set; }
    [Required]
    public string Breed { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;

}