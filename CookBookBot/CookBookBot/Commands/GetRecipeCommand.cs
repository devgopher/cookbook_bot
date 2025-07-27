using Botticelli.Framework.Commands;

namespace CookBookBot.Commands;

/// <summary>
/// Gets a recipe by id
/// </summary>
public class GetRecipeCommand : ICommand
{
    public Guid Id { get; }
}