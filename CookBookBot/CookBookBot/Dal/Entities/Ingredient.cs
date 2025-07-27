namespace CookBookBot.Dal.Entities;

public class Ingredient
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? LocalizedName { get; set; }
}