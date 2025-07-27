﻿using System.Reflection;
using Botticelli.Client.Analytics;
using Botticelli.Controls.Parsers;
using Botticelli.Framework.Commands.Processors;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.SendOptions;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.Constants;
using Botticelli.Shared.ValueObjects;
using FluentValidation;

namespace CookBookBot.Commands.Processors;

public class StartCommandProcessor<TReplyMarkup> : CommandProcessor<StartCommand> where TReplyMarkup : class
{
    private readonly SendOptionsBuilder<TReplyMarkup>? _options;
    private IBot? _bot;

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
        var responseLayout = layoutParser.ParseFromFile(Path.Combine(location, "main_layout.json"));
        var responseMarkup = layoutSupplier.GetMarkup(responseLayout);

        return responseMarkup;
    }

    public override void SetBot(IBot bot)
    {
        base.SetBot(bot);
        _bot = bot;
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
        var chatId = message.ChatIds.First();
        var greetingMessageRequest = new SendMessageRequest
        {
            Message = new Message
            {
                ChatIds = message.ChatIds,
                Body = "Bot started..."
            }
        };

        await SendMessage(greetingMessageRequest, _options, token);

        var assemblyPath = Path.GetDirectoryName(typeof(StartCommandProcessor<TReplyMarkup>).Assembly.Location) ??
                           throw new FileNotFoundException();
        // if (_bot != null)
        //     _jobManager.AddJob(_bot,
        //                        new Reliability
        //                        {
        //                            IsEnabled = false,
        //                            Delay = TimeSpan.FromSeconds(3),
        //                            IsExponential = true,
        //                            MaxTries = 5
        //                        },
        //                        new Message
        //                        {
        //                            Body = "Now you see me!",
        //                            ChatIds = [chatId],
        //                            Contact = new Contact
        //                            {
        //                                Phone = "+9003289384923842343243243",
        //                                Name = "Test",
        //                                Surname = "Botticelli"
        //                            },
        //                            Attachments =
        //                            [
        //                                new BinaryBaseAttachment(Guid.NewGuid().ToString(),
        //                                                         "testpic.png",
        //                                                         MediaType.Image,
        //                                                         string.Empty,
        //                                                         await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/testpic.png"), token)),
        //
        //                                new BinaryBaseAttachment(Guid.NewGuid().ToString(),
        //                                                         "voice.mp3",
        //                                                         MediaType.Voice,
        //                                                         string.Empty,
        //                                                         await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/voice.mp3"), token)),
        //
        //                                new BinaryBaseAttachment(Guid.NewGuid().ToString(),
        //                                                         "video.mp4",
        //                                                         MediaType.Video,
        //                                                         string.Empty,
        //                                                         await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/video.mp4"), token)),
        //
        //                                new BinaryBaseAttachment(Guid.NewGuid().ToString(),
        //                                                         "document.odt",
        //                                                         MediaType.Document,
        //                                                         string.Empty,
        //                                                         await File.ReadAllBytesAsync(Path.Combine(assemblyPath, "Media/document.odt"), token))
        //                            ]
        //                        },
        //                        new Schedule
        //                        {
        //                            Cron = "*/30 * * ? * * *"
        //                        });
    }
}