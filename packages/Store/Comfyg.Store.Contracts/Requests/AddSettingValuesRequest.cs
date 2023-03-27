using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Settings;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to add Comfyg setting values.
/// </summary>
public class AddSettingValuesRequest : AddValuesRequest<ISettingValue>
{
    /// <summary>
    /// The setting values to be added.
    /// </summary>
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISettingValue>, IEnumerable<SettingValue>, IEnumerable<IComfygValue>>))]
    public override IEnumerable<ISettingValue> Values { get; init; } = null!;
}
