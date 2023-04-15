namespace Comfyg.Cli.Extensions;

public static class EnumerableExtensions
{
    public static T? ToFlags<T>(this IEnumerable<T> flags) where T : Enum
    {
        if (flags == null) throw new ArgumentNullException(nameof(flags));

        var array = flags.ToArray();
        
        if (!array.Any()) return default;

        var result = (int) (object) array.First();
        for (var i = 1; i < array.Length; i++)
        {
            result |= (int) (object) array[i];
        }

        return (T) (object) result;
    }
}
