namespace Task1.Extensions;

public static class ZipAsync
{
    public static async IAsyncEnumerable<T[]> DoZipAsync<T>(
        this IAsyncEnumerable<T> init,
        params IAsyncEnumerable<T>[] asyncCollections)
    {
        ArgumentNullException.ThrowIfNull(init);
        ArgumentNullException.ThrowIfNull(asyncCollections);

        var query = new List<IAsyncEnumerable<T>>(1 + asyncCollections.Length) { init };
        query.AddRange(asyncCollections);

        var enumerators = query
            .Select(s => s.GetAsyncEnumerator())
            .ToList();

        try
        {
            while (true)
            {
                var asyncTuple = new T[enumerators.Count];
                for (int i = 0; i < enumerators.Count; ++i)
                {
                    if (!await enumerators[i].MoveNextAsync())
                        yield break;

                    asyncTuple[i] = enumerators[i].Current;
                }

                yield return asyncTuple;
            }
        }
        finally
        {
            foreach (IAsyncEnumerator<T> e in enumerators)
            {
                await e.DisposeAsync();
            }
        }
    }
}