public class AnimalSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AnimalType Type { get; set; }
    public List<LinkDto> Links { get; set; } = new List<LinkDto>();

}