namespace CookBookBot.Entities;

public class RecipesDataset
{
    public string? Title { get; set; }

    public string? Ingredients { get; set; }

    public string? Directions { get; set; }

    public string? Link { get; set; }

    public string? Source { get; set; }

    public string? Ner { get; set; }

    public string? Site { get; set; }

    public Guid Id { get; set; }

    public virtual Image? Image { get; set; }
}