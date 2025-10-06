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
                IEnumerable<Task<bool>> hasNextTasks = enumerators.Select(e => e.MoveNextAsync().AsTask());
                bool[] hasNextResults = await Task.WhenAll(hasNextTasks);

                if (!hasNextResults.All(success => success))
                    yield break;

                yield return enumerators.Select(e => e.Current).ToArray();
            }
        }
        finally
        {
            foreach (IAsyncEnumerator<T> e in enumerators)
            {
                await e.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}