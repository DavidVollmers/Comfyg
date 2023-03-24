using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Contracts.Settings;

namespace Comfyg.Contracts.Requests;

public class AddSettingValuesRequest : AddValuesRequest<ISettingValue>
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISettingValue>, IEnumerable<SettingValue>, IEnumerable<IComfygValue>>))]
    public override IEnumerable<ISettingValue> Values { get; init; } = null!;
}
