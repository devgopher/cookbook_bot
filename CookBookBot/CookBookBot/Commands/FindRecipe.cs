using Botticelli.Framework.Commands;

namespace CookBookBot.Commands.Processors;

/// <summary>
/// Find a recipe by keywords
/// </summary>
public class FindRecipe : ICommand
{
    public Guid Id { get; }
}