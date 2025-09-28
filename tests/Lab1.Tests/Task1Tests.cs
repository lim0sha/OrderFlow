using FluentAssertions;
using Task1.Extensions;
using Xunit;

namespace Lab1.Tests;

public class Task1Tests
{
    private static readonly int[] Sc1Elements0 = [1];
    private static readonly int[] Sc1Elements1 = [2];
    private static readonly int[] Sc1Elements2 = [3];
    private static readonly int[] Sc1Elements3 = [4];

    private static readonly int[] Sc2Item0 = [1, 2, 3];
    private static readonly int[] Sc2Item1 = [5, 6, 7];
    private static readonly int[] Sc2Item2 = [8, 9, 10];

    private static readonly int[] Sc2Item3 = [1, 5, 8];
    private static readonly int[] Sc2Item4 = [2, 6, 9];
    private static readonly int[] Sc2Item5 = [3, 7, 10];

    private static readonly int[] Sc2Item6 = [11, 12];
    private static readonly int[] Sc2Item7 = [13, 14];
    private static readonly int[] Sc2Item8 = [11, 13];
    private static readonly int[] Sc2Item9 = [12, 14];

    private static readonly int[] Sc3Item0 = [10, 20, 30, 55, 239];
    private static readonly int[] Sc3Item1 = [40, 50, 60, 100500];
    private static readonly int[] Sc3Item2 = [70, 80, 90];

    private static readonly int[] Sc3Item3 = [10, 40, 70];
    private static readonly int[] Sc3Item4 = [20, 50, 80];
    private static readonly int[] Sc3Item5 = [30, 60, 90];

    private static readonly int[] Sc3Item6 = [100, 110, 101, 104];
    private static readonly int[] Sc3Item7 = [120, 130];
    private static readonly int[] Sc3Item8 = [100, 120];
    private static readonly int[] Sc3Item9 = [110, 130];

    [Fact]
    public void Scenario1()
    {
        int[] source = [1, 2, 3, 4];

        var result = source.DoZip().ToList();

        result.Should()
            .BeEquivalentTo(
            [
                Sc1Elements0,
                Sc1Elements1,
                Sc1Elements2,
                Sc1Elements3
            ]);
    }

    [Fact]
    public async Task Scenario1Async()
    {
        static async IAsyncEnumerable<int> GetSourceAsync()
        {
            foreach (int value in new[] { 1, 2, 3, 4 })
            {
                await Task.Yield();
                yield return value;
            }
        }

        List<int[]> result = await GetSourceAsync().DoZipAsync().ToListAsync();

        result.Should()
            .BeEquivalentTo(
            [
                Sc1Elements0,
                Sc1Elements1,
                Sc1Elements2,
                Sc1Elements3
            ]);
    }

    public static IEnumerable<object[]> GetScenario2Data =>
        new List<object[]>
        {
            new object[]
            {
                Sc2Item0,
                new List<IEnumerable<int>> { Sc2Item1, Sc2Item2 },
                new List<int[]> { Sc2Item3, Sc2Item4, Sc2Item5 },
            },
            new object[]
            {
                Sc2Item6,
                new List<IEnumerable<int>> { Sc2Item7 },
                new List<int[]> { Sc2Item8, Sc2Item9 },
            },
        };

    [Theory]
    [MemberData(nameof(GetScenario2Data))]
    public void Scenario2(int[] init, IEnumerable<IEnumerable<int>> collections, IEnumerable<int[]> expected)
    {
        IEnumerable<int>[] collectionsArray = collections.ToArray();

        var result = init.DoZip(collectionsArray).ToList();

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(GetScenario2Data))]
    public async Task Scenario2Async(int[] init, IEnumerable<IEnumerable<int>> collections, IEnumerable<int[]> expected)
    {
        static async IAsyncEnumerable<int> ToAsync(IEnumerable<int> items)
        {
            foreach (int item in items)
            {
                await Task.Yield();
                yield return item;
            }
        }

        IAsyncEnumerable<int> initAsync = ToAsync(init);
        IAsyncEnumerable<int>[] collectionsAsync = collections.Select(ToAsync).ToArray();
        List<int[]> result = await initAsync.DoZipAsync(collectionsAsync).ToListAsync();

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    public static IEnumerable<object[]> GetScenario3Data =>
        new List<object[]>
        {
            new object[]
            {
                Sc3Item0,
                new List<IEnumerable<int>> { Sc3Item1, Sc3Item2 },
                new List<int[]> { Sc3Item3, Sc3Item4, Sc3Item5 },
            },
            new object[]
            {
                Sc3Item6,
                new List<IEnumerable<int>> { Sc3Item7 },
                new List<int[]> { Sc3Item8, Sc3Item9 },
            },
        };

    [Theory]
    [MemberData(nameof(GetScenario3Data))]
    public void Scenario3(int[] init, IEnumerable<IEnumerable<int>> collections, IEnumerable<int[]> expected)
    {
        IEnumerable<int>[] collectionsArray = collections.ToArray();

        var result = init.DoZip(collectionsArray).ToList();

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(GetScenario3Data))]
    public async Task Scenario3Async(int[] init, IEnumerable<IEnumerable<int>> collections, IEnumerable<int[]> expected)
    {
        static async IAsyncEnumerable<int> ToAsync(IEnumerable<int> items)
        {
            foreach (int item in items)
            {
                await Task.Yield();
                yield return item;
            }
        }

        IAsyncEnumerable<int> initAsync = ToAsync(init);
        IAsyncEnumerable<int>[] collectionsAsync = collections.Select(ToAsync).ToArray();
        List<int[]> result = await initAsync.DoZipAsync(collectionsAsync).ToListAsync();

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}