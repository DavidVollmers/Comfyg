using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Import;

internal abstract class ImportCommandBase<T> : Command where T : IComfygValue
{
    private const int MaxBatchSize = 100;

    private readonly Argument<FileInfo> _fileArgument;

    protected ImportCommandBase(string name, string? description = null) : base(name, description)
    {
        _fileArgument = new Argument<FileInfo>("INPUT_FILE",
            "The input file which will be used to import all key-value pairs from.");
        AddArgument(_fileArgument);

        this.SetHandler(HandleCommandAsync);
    }

    protected abstract AddValuesRequest<T> BuildAddValuesRequest(IEnumerable<KeyValuePair<string, string>> kvp);

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var fileArgument = context.ParseResult.GetValueForArgument(_fileArgument);

        if (!fileArgument.Exists)
            throw new FileNotFoundException("Could not find provided input file.", fileArgument.FullName);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await using var stream = fileArgument.OpenRead();

        var json = await JsonSerializer.DeserializeAsync<IDictionary<string, JsonElement>>(stream,
            cancellationToken: cancellationToken);

        if (json == null) throw new InvalidOperationException("Could not deserialize input file.");

        var batch = new List<KeyValuePair<string, string>>();

        await ImportJsonAsync(client, json, batch, cancellationToken: cancellationToken);

        if (batch.Count > 0) await ImportBatchAsync(client, batch, cancellationToken);
    }

    private async Task ImportJsonAsync(ComfygClient client, IDictionary<string, JsonElement> json,
        ICollection<KeyValuePair<string, string>> batch, string? importPath = null,
        CancellationToken cancellationToken = default)
    {
        foreach (var kvp in json)
        {
            var currentImportPath = kvp.Key;
            if (importPath != null)
            {
                if (string.IsNullOrWhiteSpace(currentImportPath)) currentImportPath = importPath;
                else currentImportPath = importPath + ":" + currentImportPath;
            }
            else if (string.IsNullOrWhiteSpace(currentImportPath))
            {
                AnsiConsole.MarkupLine($"[bold yellow]Skipping \"{currentImportPath}\" because key is empty[/]");
                continue;
            }

            switch (kvp.Value.ValueKind)
            {
                case JsonValueKind.Null:
                    AnsiConsole.MarkupLine($"[bold yellow]Skipping \"{currentImportPath}\" because value is null[/]");
                    continue;
                case JsonValueKind.Object:
                    await ImportJsonAsync(client, kvp.Value.Deserialize<IDictionary<string, JsonElement>>()!,
                        batch, currentImportPath, cancellationToken);
                    continue;
                case JsonValueKind.Undefined:
                case JsonValueKind.Array:
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                default:
                    batch.Add(new KeyValuePair<string, string>(currentImportPath, kvp.Value.ToString()));
                    break;
            }

            if (batch.Count < MaxBatchSize) continue;

            await ImportBatchAsync(client, batch, cancellationToken);

            batch.Clear();
        }
    }

    private async Task ImportBatchAsync(ComfygClient client, IEnumerable<KeyValuePair<string, string>> batch,
        CancellationToken cancellationToken)
    {
        var request = BuildAddValuesRequest(batch);

        await client.Operations<T>().AddValuesAsync(request, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully imported {request.Values.Count()} values[/]");
    }
}
