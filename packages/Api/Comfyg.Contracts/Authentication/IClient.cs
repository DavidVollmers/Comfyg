using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Comfyg.Contracts.Authentication;

public interface IClient
{
    [Required] string ClientId { get; }

    [JsonIgnore] [IgnoreDataMember] string ClientSecret { get; }

    [Required] [MaxLength(32)] string FriendlyName { get; }
}