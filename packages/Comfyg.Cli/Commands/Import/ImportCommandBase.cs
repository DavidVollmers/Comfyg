using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Comfyg.Cli.Extensions;
using Comfyg.Client;
using Comfyg.Contracts;
using Comfyg.Contracts.Requests;
using Spectre.Console;

namespace Comfyg.Cli.Commands.Import;

public abstract class ImportCommandBase<T> : Command where T : IComfygValue
{
    private const int MaxBatchSize = 100;

    private readonly Argument<FileInfo> _fileArgument;

    protected ImportCommandBase(string name, string? description = null) : base(name, description)
    {
        _fileArgument = new Argument<FileInfo>("file", "The JSON file containing the comfyg values to import");
        AddArgument(_fileArgument);

        this.SetHandler(HandleCommandAsync);
    }

    protected abstract AddValuesRequest<T> BuildAddValuesRequest(IEnumerable<KeyValuePair<string, string>> kvp);

    private async Task HandleCommandAsync(InvocationContext context)
    {
        var fileArgument = context.ParseResult.GetValueForArgument(_fileArgument);

        if (!fileArgument.Exists)
            throw new FileNotFoundException("Could not find JSON file to import.", fileArgument.FullName);

        var cancellationToken = context.GetCancellationToken();

        using var client = await State.User.RequireClientAsync(cancellationToken);

        await using var stream = fileArgument.OpenRead();

        var json = await JsonSerializer.DeserializeAsync<IDictionary<string, object?>>(stream,
            cancellationToken: cancellationToken);

        if (json == null) throw new InvalidOperationException("Could not deserialize JSON file.");

        await ImportJsonAsync(client, json, cancellationToken: cancellationToken);
    }

    private async Task ImportJsonAsync(ComfygClient client, IDictionary<string, object?> json,
        string? importPath = null, IList<KeyValuePair<string, string>>? batch = null,
        CancellationToken cancellationToken = default)
    {
        batch ??= new List<KeyValuePair<string, string>>();

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

            switch (kvp.Value)
            {
                case null:
                    AnsiConsole.MarkupLine($"[bold yellow]Skipping \"{currentImportPath}\" because value is null[/]");
                    continue;
                case IDictionary<string, object?> subJson:
                    await ImportJsonAsync(client, subJson, currentImportPath, batch, cancellationToken);
                    continue;
            }

            batch.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value.ToString()!));

            if (batch.Count < MaxBatchSize) continue;

            await ImportBatchAsync(client, batch, cancellationToken);

            batch.Clear();
        }

        if (batch.Count > 0) await ImportBatchAsync(client, batch, cancellationToken);
    }

    private async Task ImportBatchAsync(ComfygClient client, IEnumerable<KeyValuePair<string, string>> batch,
        CancellationToken cancellationToken)
    {
        var request = BuildAddValuesRequest(batch);

        await client.Operations<T>().AddValuesAsync(request, cancellationToken);

        AnsiConsole.MarkupLine($"[bold green]Successfully imported {request.Values.Count()} values[/]");
    }
}
