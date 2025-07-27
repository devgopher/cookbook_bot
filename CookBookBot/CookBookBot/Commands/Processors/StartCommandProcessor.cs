using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Controls.Parsers;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.SendOptions;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace CookBookBot.Commands.Processors;

public class StartCommandProcessor<TReplyMarkup> : CommandProcessor<StartCommand> where TReplyMarkup : class
{
    private readonly SendOptionsBuilder<TReplyMarkup>? _options;

    public StartCommandProcessor(ILogger<StartCommandProcessor<TReplyMarkup>> logger,
        ICommandValidator<StartCommand> commandValidator,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser,
        IValidator<Message> messageValidator)
        : base(logger,
            commandValidator,
            messageValidator)
    {
        var responseMarkup = Init(layoutSupplier, layoutParser);

        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }

    public StartCommandProcessor(ILogger<StartCommandProcessor<TReplyMarkup>> logger,
        ICommandValidator<StartCommand> commandValidator,
        ILayoutSupplier<TReplyMarkup> layoutSupplier,
        ILayoutParser layoutParser,
        IValidator<Message> messageValidator,
        MetricsProcessor? metricsProcessor)
        : base(logger,
            commandValidator,
            messageValidator,
            metricsProcessor)
    {
        var responseMarkup = Init(layoutSupplier, layoutParser);

        _options = SendOptionsBuilder<TReplyMarkup>.CreateBuilder(responseMarkup);
    }

    private static TReplyMarkup Init(ILayoutSupplier<TReplyMarkup> layoutSupplier, ILayoutParser layoutParser)
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "start_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);

        return responseMarkup;
    }

    protected override Task InnerProcessContact(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessPoll(Message message, CancellationToken token) => Task.CompletedTask;

    protected override Task InnerProcessLocation(Message message, CancellationToken token) => Task.CompletedTask;

    protected override async Task InnerProcess(Message message, CancellationToken token)
    {
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                ChatIds = message.ChatIds,
                Body = "Bot started..."
            }
        };

        await SendMessage(greetingMessageRequest, _options, token);
    }
}