using Comfyg.Contracts.Changes;

namespace Comfyg.Contracts.Responses;

public sealed class GetDiffResponse
{
    public IEnumerable<IChangeLog> ChangeLog { get; set; }

    public GetDiffResponse(IEnumerable<IChangeLog> changeLog)
    {
        ChangeLog = changeLog ?? throw new ArgumentNullException(nameof(changeLog));
    }
}