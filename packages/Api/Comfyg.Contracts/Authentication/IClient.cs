using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Comfyg.Contracts.Authentication;

public interface IClient
{
    string ClientId { get; }

    [JsonIgnore] [IgnoreDataMember] string ClientSecret { get; }

    string FriendlyName { get; }
}