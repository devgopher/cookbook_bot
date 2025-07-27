using Botticelli.Client.Analytics;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace CookBookBot.Commands.Processors;

public class FindRecipeCommandProcessor : WaitForClientResponseCommandChainProcessor<FindRecipeCommand>
{
    public FindRecipeCommandProcessor(ILogger<CommandChainProcessor<FindRecipeCommand>> logger,
        ICommandValidator<FindRecipeCommand> commandValidator, MetricsProcessor metricsProcessor,
        IValidator<Message> messageValidator) : base(logger, commandValidator, metricsProcessor, messageValidator)
    {
    }

    protected override Task InnerProcessContact(Message message, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    protected override Task InnerProcessPoll(Message message, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    protected override Task InnerProcessLocation(Message message, CancellationToken token)
    {
        return Task.CompletedTask;
    }

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var responseMessage = new Message
        {
            ChatIdInnerIdLinks = message.ChatIdInnerIdLinks,
            ChatIds = message.ChatIds,
            Subject = string.Empty,
            Body = "Enter ingredients (for example: \"egg garlic\")"
        };

        await SendMessage(responseMessage, token);
    }
}