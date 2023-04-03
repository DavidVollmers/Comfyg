using System.CommandLine;
using Comfyg.Store.Contracts;

namespace Comfyg.Cli.Commands.Set;

internal abstract class SetValuePermissionsCommandBase<T> : Command where T : IComfygValue
{
    private readonly Argument<string> _keyArgument;
    private readonly Argument<string> _clientIdArgument;

    protected SetValuePermissionsCommandBase(string name, string? description = null) : base(name, description)
    {
        _keyArgument = new Argument<string>("KEY", "The key of the key-value pair to set the permission for.");
        AddArgument(_keyArgument);
        
        _clientIdArgument = new Argument<string>("CLIENT_ID", "The ID of the client to set the permission for.");
        AddArgument(_clientIdArgument);
    }
}
