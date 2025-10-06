using FluentAssertions;
using NSubstitute;
using Task2.Implementations;
using Task2.Interfaces;
using Task2.Models;
using Xunit;

namespace Lab1.Tests;

public class Task2Tests
{
    [Fact]
    public async Task Scenario1()
    {
        var request = new RequestModel("method", [1, 2, 3]);
        byte[] response = [3, 2, 1];
        CancellationToken ct = CancellationToken.None;
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);

        mockService.When(sCall =>
                sCall.BeginOperation(
                    Arg.Any<Guid>(),
                    request,
                    ct))
            .Do(callback =>
            {
                Guid guid = callback.Arg<Guid>();
                Task.Delay(1000, ct)
                    .ContinueWith(_ => client.HandleOperationResult(guid, response), ct);
            });
        Task<ResponseModel> task = client.SendAsync(request, ct);
        ResponseModel final = await task;

        final.Should().NotBeNull();
        final.Data.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Scenario2()
    {
        var request = new RequestModel("method", [1, 2, 3]);
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);
        Guid id = Guid.Empty;

        mockService.When(sCall =>
                sCall.BeginOperation(
                    Arg.Any<Guid>(),
                    Arg.Any<RequestModel>(),
                    Arg.Any<CancellationToken>()))
            .Do(callback => id = callback.ArgAt<Guid>(0));
        Task<ResponseModel> task = client.SendAsync(request, CancellationToken.None);
        var ex = new InvalidOperationException("exception");
        client.HandleOperationError(id, ex);

        task.Should().NotBeNull();
        await task.Invoking(async t => await t)
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("exception");
    }

    [Fact]
    public async Task Scenario3()
    {
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var request = new RequestModel("method", [1, 2, 3]);
        Task<ResponseModel> task = client.SendAsync(request, cts.Token);

        task.Should().NotBeNull();
        await task.Invoking(async taskAsync => await taskAsync)
            .Should()
            .ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task Scenario4()
    {
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);
        var request = new RequestModel("method", [1, 2, 3]);
        var cts = new CancellationTokenSource();

        mockService.When(sCall =>
                sCall.BeginOperation(
                    Arg.Any<Guid>(),
                    Arg.Any<RequestModel>(),
                    Arg.Any<CancellationToken>()))
            .Do(callback => callback.ArgAt<Guid>(0));
        Task<ResponseModel> task = client.SendAsync(request, cts.Token);
        await cts.CancelAsync();

        task.Should().NotBeNull();
        await task.Invoking(async t => await t)
            .Should()
            .ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task Scenario5()
    {
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);
        var request = new RequestModel("method", [1, 2, 3]);

        mockService.When(sCall =>
                sCall.BeginOperation(
                    Arg.Any<Guid>(),
                    Arg.Any<RequestModel>(),
                    Arg.Any<CancellationToken>()))
            .Do(callback =>
            {
                Guid id = callback.ArgAt<Guid>(0);
                client.HandleOperationResult(id, [4]);
            });
        ResponseModel result = await client.SendAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().Equal(new byte[] { 4 });
    }

    [Fact]
    public async Task Scenario6()
    {
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);
        var request = new RequestModel("method", [1, 2, 3]);

        mockService.When(sCall =>
                sCall.BeginOperation(
                    Arg.Any<Guid>(),
                    Arg.Any<RequestModel>(),
                    Arg.Any<CancellationToken>()))
            .Do(callback =>
            {
                Guid id = callback.ArgAt<Guid>(0);
                client.HandleOperationError(id, new InvalidOperationException("exception"));
            });
        Task<ResponseModel> task = client.SendAsync(request, CancellationToken.None);

        task.Should().NotBeNull();
        await task.Invoking(async t => await t)
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("exception");
    }

    [Fact]
    public async Task Scenario7()
    {
        ILibraryOperationService mockService = Substitute.For<ILibraryOperationService>();
        var client = new RequestClient(mockService);
        var request = new RequestModel("method", [1, 2, 3]);
        var cts = new CancellationTokenSource();

        mockService.When(sCall => sCall.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(_ => cts.CancelAsync());
        Task<ResponseModel> task = client.SendAsync(request, cts.Token);

        task.Should().NotBeNull();
        await task.Invoking(async t => await t)
            .Should()
            .ThrowAsync<TaskCanceledException>();
    }
}