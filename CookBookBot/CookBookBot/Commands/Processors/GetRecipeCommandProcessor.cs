using System.Text;
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
        IValidator<Message> messageValidator, CookBookDbContext context) : base(logger,
        commandValidator,
        metricsProcessor,
        messageValidator)
    {
        _context = context;
    }

    public override async Task ProcessAsync(Message message, CancellationToken token)
    {
        if (message.ProcessingArgs == null || !message.ProcessingArgs.Any())
            return;

        var ingredients = message.ProcessingArgs[0].Split(" ").Select(i => i.ToLowerInvariant());

        var ingredientIds = _context.Ingredients.Where(i => ingredients.Contains(i.Name)).Select(i => i.Id).ToArray();

        var recipeIds = _context.Ingredient2Recipes.Where(i2R => ingredientIds.Contains(i2R.IngredientId))
            .Select(l => l.RecipeId).Distinct();

        var recipes = _context.RecipesDatasets.Where(r => recipeIds.Contains(r.Id));

        message.ProcessingArgs = [];

        if (!recipes.Any())
        {
            message.Body = "🚫 No recipes were found!";
            
            await SendMessage(message, token);
        }
        
        foreach (var recipe in recipes.AsEnumerable().OrderBy(o => Random.Shared.Next()).Take(3))
        {
            var sb = new StringBuilder();
            sb.Append($"\u2714\u2714 {recipe.Title} \u2714\u2714\n");
            sb.Append("\nIngredients: \n");
            sb.AppendJoin('\n', recipe.Ingredients?.Replace("[", string.Empty).Replace("]", string.Empty)
                .Replace("\"", string.Empty).Split(",").Select(i => $"\u2705 {i};")!);
            sb.Append("\nSteps: \n");
            sb.AppendJoin('\n', recipe.Directions?.Replace("[", string.Empty).Replace("]", string.Empty)
                .Replace("\"", string.Empty).Split(",").Select(d => $"\u2705  {d};")!);
            sb.Append($"\n{recipe.Link}\n\n");
            message.Body = sb.ToString();
        
            await SendMessage(message, token);
        }
    }

    protected override Task InnerProcess(Message message, CancellationToken token) => Task.FromResult(Task.CompletedTask);
}