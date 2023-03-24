using Comfyg.Client;

namespace Comfyg.Cli.Extensions;

internal static class StateExtensions
{
    public static async Task<ComfygClient> RequireClientAsync(this State state, CancellationToken cancellationToken)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));

        var connectionString = await state.ReadAsync<string>(nameof(Comfyg), nameof(ComfygClient), cancellationToken)
            .ConfigureAwait(false);

        if (connectionString == null)
            throw new InvalidOperationException("No connection established. Please use the connect command.");

        return new ComfygClient(connectionString);
    }
}
