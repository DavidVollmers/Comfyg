using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Configuration;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Store.Api.Requests;

public sealed class AddConfigurationValuesRequest : IAddValuesRequest<IConfigurationValue>
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<IConfigurationValue>, IEnumerable<ConfigurationValue>,
            IEnumerable<IComfygValue>>))]
    public IEnumerable<IConfigurationValue> Values { get; init; } = null!;
}
