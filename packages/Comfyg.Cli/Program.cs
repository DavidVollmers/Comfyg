﻿using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Comfyg.Cli;
using Comfyg.Cli.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddComfygCommands(); })
    .Build();

var rootCommand = new RootCommand("Comfyg Command-Line Interface") { Name = "comfyg" };
rootCommand.AddOption(new Option<bool>("--nocheck", "If set the CLI will not check for updates on startup."));

var commands = host.Services.GetServices<Command>();
foreach (var command in commands)
{
    rootCommand.Add(command);
}

if (args.Length == 0)
{
    AnsiConsole.Write(new FigletText(nameof(Comfyg)).Color(CliConstants.PrimaryColor));
}

if (args.All(a => a != "--nocheck"))
{
    try
    {
        await Cli.CheckForUpdateAsync();
    }
    catch (Exception e)
    {
        AnsiConsole.MarkupLine("[bold red]Could not check for updates:[/]");
        AnsiConsole.WriteException(e, ExceptionFormats.ShortenEverything);
    }
}

var parser = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseExceptionHandler((e, context) =>
    {
        AnsiConsole.WriteException(e, ExceptionFormats.ShortenEverything);
        context.ExitCode = 1;
    })
    .Build();

return await parser.InvokeAsync(args);
