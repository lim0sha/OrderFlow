namespace Task1.Extensions;

public static class Zip
{
    public static IEnumerable<T[]> DoZip<T>(
        this IEnumerable<T> init,
        params IEnumerable<T>[] collections)
    {
        var query = new List<IEnumerable<T>>(1 + collections.Length) { init };
        query.AddRange(collections);

        var enumerators = query
            .Select(s => s.GetEnumerator())
            .ToList();

        try
        {
            while (enumerators.All(x => x.MoveNext()))
            {
                yield return enumerators.Select(x => x.Current).ToArray();
            }
        }
        finally
        {
            foreach (IEnumerator<T> e in enumerators)
            {
                e.Dispose();
            }
        }
    }
}