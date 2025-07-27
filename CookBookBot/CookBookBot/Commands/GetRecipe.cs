using Botticelli.Framework.Commands;

namespace CookBookBot.Commands;

/// <summary>
/// Gets a recipe by id
/// </summary>
public class GetRecipe : ICommand
{
    public Guid Id { get; }
}