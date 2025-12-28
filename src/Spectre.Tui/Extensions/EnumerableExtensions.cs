namespace Spectre.Tui;

internal static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public void ForEach(Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public IEnumerable<(int Index, bool First, bool Last, T Item)> Enumerate()
        {
            ArgumentNullException.ThrowIfNull(source);
            return Enumerate(source.GetEnumerator());
        }
    }

    private static IEnumerable<(int Index, bool First, bool Last, T Item)> Enumerate<T>(IEnumerator<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var first = true;
        var last = !source.MoveNext();

        for (var index = 0; !last; index++)
        {
            var current = source.Current;
            last = !source.MoveNext();
            yield return (index, first, last, current);
            first = false;
        }
    }
}