namespace Comfyg.Client;

public static class ByteEnumerableExtensions
{
    public static bool StartsWith(this IReadOnlyList<byte> bytes, IReadOnlyCollection<byte> pattern)
    {
    }

    private static bool IsMatch(IReadOnlyList<byte> bytes, IReadOnlyCollection<byte> pattern, int position)
    {
        if (pattern.Count > (bytes.Count - position))
            return false;

        return !pattern.Where((t, i) => bytes[position + i] != t).Any();
    }
}
