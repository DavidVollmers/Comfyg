using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Api.Models;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Settings;

namespace Comfyg.Store.Api.Requests;

public sealed class AddSettingValuesRequest : IAddValuesRequest<ISettingValue>
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISettingValue>, IEnumerable<SettingValueModel>,
            IEnumerable<IComfygValue>>))]
    public IEnumerable<ISettingValue> Values { get; init; } = null!;
}
