﻿namespace Comfyg.Client;

internal static class ByteEnumerableExtensions
{
    public static bool StartsWith(this IReadOnlyList<byte> bytes, IReadOnlyCollection<byte> pattern)
    {
        return IsMatch(bytes, pattern, 0);
    }

    private static bool IsMatch(IReadOnlyList<byte> bytes, IReadOnlyCollection<byte> pattern, int position)
    {
        if (pattern.Count > (bytes.Count - position))
            return false;

        return !pattern.Where((t, i) => bytes[position + i] != t).Any();
    }
}
