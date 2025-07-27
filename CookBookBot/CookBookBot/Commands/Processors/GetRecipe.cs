using Botticelli.Framework.Commands;

namespace CookBookBot.Commands.Processors;

/// <summary>
/// Gets a recipe by id
/// </summary>
public class GetRecipe : ICommand
{
    public Guid Id { get; }
}