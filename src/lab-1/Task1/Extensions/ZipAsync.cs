using System.Runtime.CompilerServices;

namespace Task1.Extensions;

public static class ZipAsync
{
    public static async IAsyncEnumerable<T[]> DoZipAsync<T>(
        this IAsyncEnumerable<T> init,
        [EnumeratorCancellation] CancellationToken cancellationToken = default,
        params IAsyncEnumerable<T>[] asyncCollections)
    {
        var query = new List<IAsyncEnumerable<T>>(1 + asyncCollections.Length) { init };
        query.AddRange(asyncCollections);

        var enumerators = query
            .Select(s => s.GetAsyncEnumerator(cancellationToken))
            .ToList();

        try
        {
            while (true)
            {
                IEnumerable<Task<bool>> moveNextTasks = enumerators.Select(e => e.MoveNextAsync().AsTask());
                bool[] results = await Task.WhenAll(moveNextTasks).ConfigureAwait(false);
                if (!results.All(success => success))
                    yield break;
                yield return enumerators.Select(e => e.Current).ToArray();
            }
        }
        finally
        {
            foreach (IAsyncEnumerator<T> enumerator in enumerators)
            {
                await enumerator.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}