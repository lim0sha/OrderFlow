using Task1.Models.Entities;
using Task2.Sources.Providers;
using Xunit;

namespace Lab2.Tests;

public class Task2Tests
{
    [Fact]
    public async Task Scenario1()
    {
        var provider = new ConfigStorage();
        var input = new List<Config> { new() { Key = "A", Value = "1" } };
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        await provider.Refresh(ToAsyncEnumerable(input), CancellationToken.None);
        Assert.True(provider.TryGet("A", out string? value) && value == "1");
        Assert.True(reloaded);
    }

    [Fact]
    public async Task Scenario2()
    {
        var provider = new ConfigStorage();
        var initial = new List<Config> { new() { Key = "A", Value = "1" } };
        await provider.Refresh(ToAsyncEnumerable(initial), CancellationToken.None);
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        await provider.Refresh(ToAsyncEnumerable(initial), CancellationToken.None);
        Assert.False(reloaded);
        Assert.True(provider.TryGet("A", out string? value) && value == "1");
    }

    [Fact]
    public async Task Scenario3()
    {
        var provider = new ConfigStorage();
        var initial = new List<Config> { new() { Key = "A", Value = "1" } };
        await provider.Refresh(ToAsyncEnumerable(initial), CancellationToken.None);
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        var updated = new List<Config> { new() { Key = "A", Value = "2" } };
        await provider.Refresh(ToAsyncEnumerable(updated), CancellationToken.None);
        Assert.True(reloaded);
        Assert.True(provider.TryGet("A", out string? value) && value == "2");
    }

    [Fact]
    public async Task Scenario4()
    {
        var provider = new ConfigStorage();
        var initial = new List<Config> { new() { Key = "A", Value = "1" } };
        await provider.Refresh(ToAsyncEnumerable(initial), CancellationToken.None);
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        var empty = new List<Config>();
        await provider.Refresh(ToAsyncEnumerable(empty), CancellationToken.None);
        Assert.True(reloaded);
        Assert.Empty(provider.GetChildKeys([], string.Empty));
    }

    private static async IAsyncEnumerable<Config> ToAsyncEnumerable(IEnumerable<Config> configs)
    {
        foreach (Config config in configs)
            yield return config;
        await Task.CompletedTask;
    }
}