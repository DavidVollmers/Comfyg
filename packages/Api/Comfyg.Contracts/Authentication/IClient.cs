using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Comfyg.Contracts.Authentication;

public interface IClient
{
    [Required] string ClientId { get; }

    [JsonIgnore]
    [ValidateNever]
    [IgnoreDataMember]
    string ClientSecret { get; }

    [Required] [MaxLength(32)] string FriendlyName { get; }
}