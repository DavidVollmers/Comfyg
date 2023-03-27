using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Comfyg.Cli.Extensions;
using Comfyg.Store.Contracts;
using Newtonsoft.Json;
using Spectre.Console;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Comfyg.Cli.Commands.Export;

internal abstract class ExportCommandBase<T> : Command where T : IComfygValue
{
    private readonly Argument<FileInfo> _fileArgument;

    protected ExportCommandBase(string name, string? description = null) : base(name, description)
    {
        _fileArgument = new Argument<FileInfo>("OUTPUT_FILE",
            "The output file which will be used to export key-value pairs into.");
        AddArgument(_fileArgument);

        this.SetHandler(HandleCommandAsync);
    }

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var fileArgument = context.ParseResult.GetValueForArgument(_fileArgument);

        if (fileArgument.Exists)
        {
            var overwrite =
                AnsiConsole.Prompt(
                    new ConfirmationPrompt(
                        "[bold yellow]The provided file already exists. Do you want to overwrite it?[/]"));
            if (!overwrite) return;
        }

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        var values = client.Operations<T>().GetValuesAsync(cancellationToken: cancellationToken);

        var count = 0;
        var json = new Dictionary<string, object>();
        await foreach (var value in values.WithCancellation(cancellationToken))
        {
            WriteValue(json, value.Key.Split(":"), value.Value);

            ++count;
        }

        await using var stream = fileArgument.OpenWrite();
        await JsonSerializer.SerializeAsync(stream, json, new JsonSerializerOptions { WriteIndented = true },
            cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully exported {count} values[/]");
    }

    private static void WriteValue(IDictionary<string, object> json, IReadOnlyList<string> path, string value)
    {
        while (true)
        {
            if (path.Count == 1)
            {
                json.Add(path[0], value);
                return;
            }

            if (json.TryGetValue(path[0], out var existing))
            {
                if (existing is string existingValue)
                {
                    json[path[0]] = new Dictionary<string, object> { { string.Empty, existingValue } };
                }
            }
            else
            {
                json.Add(path[0], new Dictionary<string, object>());
            }

            json = (IDictionary<string, object>)json[path[0]];
            path = path.Skip(1).ToArray();
        }
    }
}
