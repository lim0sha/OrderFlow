namespace Task1.Extensions;

public static class Zip
{
    public static IEnumerable<T[]> DoZip<T>(
        this IEnumerable<T> init,
        params IEnumerable<T>[] collections)
    {
        ArgumentNullException.ThrowIfNull(init);
        ArgumentNullException.ThrowIfNull(collections);

        var query = new List<IEnumerable<T>>(1 + collections.Length) { init };
        query.AddRange(collections);

        var enumerators = query
            .Select(s => s.GetEnumerator())
            .ToList();

        try
        {
            while (true)
            {
                var tuple = new T[enumerators.Count];
                for (int i = 0; i < enumerators.Count; ++i)
                {
                    if (!enumerators[i].MoveNext())
                        yield break;

                    tuple[i] = enumerators[i].Current;
                }

                yield return tuple;
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