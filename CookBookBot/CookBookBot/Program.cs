using Botticelli.Client.Analytics.Extensions;
using Botticelli.Controls.Parsers;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using Botticelli.Interfaces;
using CookBookBot.Commands;
using CookBookBot.Commands.Processors;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

var bot = builder.Services
       .AddTelegramBot(builder.Configuration)
       .Build();

builder.Services
       .AddTelegramLayoutsSupport()
       .AddLogging(cfg => cfg.AddNLog())
       .AddSingleton<IBot>(bot)
       .AddSingleton<ILayoutParser, JsonLayoutParser>()
       .AddSingleton<GetRecipeCommandProcessor>()
       .AddSingleton<FindRecipeCommandProcessor>()
       .AddSingleton<StartCommandProcessor<ReplyKeyboardMarkup>>()
       .AddSingleton<StopCommandProcessor<ReplyKeyboardMarkup>>()
       .AddSingleton<InfoCommandProcessor<ReplyKeyboardMarkup>>()
       .AddScoped<ICommandValidator<InfoCommand>, PassValidator<InfoCommand>>()
       .AddScoped<ICommandValidator<StartCommand>, PassValidator<StartCommand>>()
       .AddScoped<ICommandValidator<StopCommand>, PassValidator<StopCommand>>()
       .AddScoped<ICommandValidator<FindRecipeCommand>, PassValidator<FindRecipeCommand>>();

builder.Services.AddBotCommand<InfoCommand>()
       .AddProcessor<InfoCommandProcessor<ReplyKeyboardMarkup>>()
       .AddValidator<PassValidator<InfoCommand>>();

builder.Services.AddBotCommand<StartCommand>()
       .AddProcessor<StartCommandProcessor<ReplyKeyboardMarkup>>()
       .AddValidator<PassValidator<StartCommand>>();

builder.Services.AddBotCommand<StopCommand>()
       .AddProcessor<StopCommandProcessor<ReplyKeyboardMarkup>>()
       .AddValidator<PassValidator<StopCommand>>();

builder.Services.AddBotChainProcessedCommand<FindRecipeCommand, PassValidator<FindRecipeCommand>>()
       .AddNext<FindRecipeCommandProcessor>()
       .AddNext<GetRecipeCommandProcessor>();

var app = builder.Build();
app.Services.RegisterBotChainedCommand<FindRecipeCommand, TelegramBot>();

await app.RunAsync();
