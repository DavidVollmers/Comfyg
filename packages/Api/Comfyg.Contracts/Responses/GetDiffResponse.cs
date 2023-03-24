using System.Text.Json.Serialization;
using Comfyg.Contracts.Changes;

namespace Comfyg.Contracts.Responses;

public sealed class GetDiffResponse
{
    [JsonConverter(typeof(ContractConverter<IEnumerable<IChangeLog>, IEnumerable<ChangeLog>>))]
    public IEnumerable<IChangeLog> ChangeLog { get; }

    public GetDiffResponse(IEnumerable<IChangeLog> changeLog)
    {
        ChangeLog = changeLog ?? throw new ArgumentNullException(nameof(changeLog));
    }
}
