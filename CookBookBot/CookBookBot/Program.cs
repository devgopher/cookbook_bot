using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
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
       .AddSingleton<IBot>(bot);

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
       .AddNext<FindRecipeCommandProcessor<ReplyKeyboardMarkup>>()
       .AddNext<GetRecipeCommandProcessor<ReplyKeyboardMarkup>>();


await builder.Build().RunAsync();