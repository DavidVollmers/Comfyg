using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts;

/// <summary>
/// A Comfyg client used for authentication and authorization.
/// </summary>
[JsonConverter(typeof(ContractConverter<IClient, Implementation>))]
public interface IClient
{
    /// <summary>
    /// The client ID.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [TechnicalIdentifier]
    string ClientId { get; }

    /// <summary>
    /// The client secret.
    /// </summary>
    [JsonIgnore]
    [IgnoreDataMember]
    string ClientSecret { get; }

    /// <summary>
    /// The user friendly display name of the client.
    /// </summary>
    [Required]
    [MaxLength(256)]
    string FriendlyName { get; }

    /// <summary>
    /// Specifies if the client uses an asymmetric client secret.
    /// </summary>
    bool IsAsymmetric { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IClient
    {
        public string ClientId { get; init; } = null!;

        public string ClientSecret { get; init; } = null!;

        public string FriendlyName { get; init; } = null!;

        public bool IsAsymmetric { get; init; }
    }
}
