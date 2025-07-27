namespace CookBookBot.Entities;

public class Image
{
    public Guid Id { get; set; }

    public string? Image1 { get; set; }

    public Guid RecipeId { get; set; }

    public virtual RecipesDataset IdNavigation { get; set; } = null!;
}