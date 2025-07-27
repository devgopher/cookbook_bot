using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Shared.ValueObjects;
using CookBookBot.Dal;
using FluentValidation;

namespace CookBookBot.Commands.Processors;

public class GetRecipeCommandProcessor : CommandChainProcessor<FindRecipeCommand>
{
    private readonly CookBookDbContext _context;

    public GetRecipeCommandProcessor(ILogger<CommandChainProcessor<FindRecipeCommand>> logger,
        ICommandValidator<FindRecipeCommand> commandValidator,
        MetricsProcessor metricsProcessor,
        IValidator<Message> messageValidator) : base(logger,
        commandValidator,
        metricsProcessor,
        messageValidator)
    {
    }

    public override async Task ProcessAsync(Message message, CancellationToken token)
    {
        if (message.ProcessingArgs == null || !message.ProcessingArgs.Any())
            return;

        var ingredients = message.ProcessingArgs[0].Split(" ").Select(i => i.ToLowerInvariant());

        var ingredientIds = _context.Ingredients.Where(i => ingredients.Contains(i.Name)).Select(i => i.Id);

        var recipeIds = _context.Ingredient2recipes.Where(i2R => ingredientIds.Contains(i2R.IngredientId))
            .Select(l => l.RecipeId).Distinct();

        var recipes = _context.RecipesDatasets.Where(r => recipeIds.Contains(r.Id));

        message.ProcessingArgs = [];

        message.Body = "See results: \n";

        foreach (var recipe in recipes.OrderBy(o => Random.Shared.Next()).Take(3))
        {
            message.Body += $"\\U00002714\\U00002714 {recipe.Title} \\U00002714\\U00002714\n";
            message.Body += "\nIngredients: \n";
            message.Body += recipe.Ingredients?.Replace("[", string.Empty).Replace("]", string.Empty)
                .Replace("\"", string.Empty).Split(",").Select(i => $"\\U2705 {i};");
            message.Body += "\nSteps: \n";
            message.Body += recipe.Directions?.Replace("[", string.Empty).Replace("]", string.Empty)
                .Replace("\"", string.Empty).Split(",").Select(d => $"\\U2705  {d};\n");
            message.Body += $"\n{recipe.Link}\n\n";
        }
        
        await SendMessage(message, token);
    }

    protected override Task InnerProcess(Message message, CancellationToken token) => Task.FromResult(Task.CompletedTask);
}