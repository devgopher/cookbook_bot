using Botticelli.Framework.Commands;

namespace CookBookBot.Commands;

/// <summary>
/// Find a recipe by keywords
/// </summary>
public class FindRecipeCommand : ICommand
{
    public Guid Id { get; }
}