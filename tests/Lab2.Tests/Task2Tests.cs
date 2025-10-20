using Task1.Models.Entities;
using Task2.Sources.Providers;
using Xunit;

namespace Lab2.Tests;

public class Task2Tests
{
    [Fact]
    public void Scenario1()
    {
        var provider = new ConfigStorage();
        var input = new List<Config> { new() { Key = "A", Value = "1" } };
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        provider.Refresh(input.ToArray());
        Assert.True(provider.TryGet("A", out string? value) && value == "1");
        Assert.True(reloaded);
    }

    [Fact]
    public void Scenario2()
    {
        var provider = new ConfigStorage();
        var initial = new List<Config> { new() { Key = "A", Value = "1" } };
        provider.Refresh(initial);
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        provider.Refresh(initial);
        Assert.False(reloaded);
        Assert.True(provider.TryGet("A", out string? value) && value == "1");
    }

    [Fact]
    public void Scenario3()
    {
        var provider = new ConfigStorage();
        var initial = new List<Config> { new() { Key = "A", Value = "1" } };
        provider.Refresh(initial);
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        var updated = new List<Config> { new() { Key = "A", Value = "2" } };
        provider.Refresh(updated);
        Assert.True(reloaded);
        Assert.True(provider.TryGet("A", out string? value) && value == "2");
    }

    [Fact]
    public void Scenario4()
    {
        var provider = new ConfigStorage();
        var initial = new List<Config> { new() { Key = "A", Value = "1" } };
        provider.Refresh(initial);
        bool reloaded = false;
        provider.ReloadTokenAccessor.RegisterChangeCallback(_ => reloaded = true, null);
        var empty = new List<Config>();
        provider.Refresh(empty);
        Assert.True(reloaded);
        Assert.Empty(provider.GetChildKeys([], string.Empty));
    }
}