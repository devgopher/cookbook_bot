using Botticelli.Controls.Parsers;
using Botticelli.Framework.Commands.Validators;
using Botticelli.Framework.Extensions;
using Botticelli.Framework.Telegram;
using Botticelli.Framework.Telegram.Extensions;
using CookBookBot.Commands;
using CookBookBot.Commands.Processors;
using CookBookBot.Dal;
using CookBookBot.Settings;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddTelegramBot(builder.Configuration)
       .Prepare();

builder.Services
       .AddTelegramLayoutsSupport()
       .AddLogging(cfg => cfg.AddNLog())
       .AddSingleton<ILayoutParser, JsonLayoutParser>()
       .AddSingleton<StartCommandProcessor<ReplyKeyboardMarkup>>()
       .AddSingleton<StopCommandProcessor<ReplyKeyboardMarkup>>()
       .AddSingleton<InfoCommandProcessor<ReplyKeyboardMarkup>>()
       .AddScoped<ICommandValidator<InfoCommand>, PassValidator<InfoCommand>>()
       .AddScoped<ICommandValidator<StartCommand>, PassValidator<StartCommand>>()
       .AddScoped<ICommandValidator<StopCommand>, PassValidator<StopCommand>>()
       .AddScoped<ICommandValidator<FindRecipeCommand>, PassValidator<FindRecipeCommand>>();

var connection = builder.Configuration.GetSection(CookBookSettings.Section).Get<CookBookSettings>()?.DbConnection;

builder.Services.AddDbContext<CookBookDbContext>(opt =>
       opt.UseNpgsql(connection)
              .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Singleton);

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
       .AddNext<GetRecipeCommandProcessor<ReplyKeyboardMarkup>>();

var app = builder.Build();
app.Services.RegisterBotChainedCommand<FindRecipeCommand, TelegramBot>()
       .UseTelegramBot();

await app.RunAsync();
