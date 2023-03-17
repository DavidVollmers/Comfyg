using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Contracts.Settings;

namespace Comfyg.Contracts.Requests;

public class AddSettingValuesRequest
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISettingValue>, IEnumerable<SettingValue>, IEnumerable<IComfygValue>>))]
    public IEnumerable<ISettingValue> SettingValues { get; set; } = null!;
}