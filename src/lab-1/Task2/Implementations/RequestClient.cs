using System.Collections.Concurrent;
using Task2.Interfaces;
using Task2.Models;

namespace Task2.Implementations;

public class RequestClient : IRequestClient, ILibraryOperationHandler, ILibraryOperationRefuser
{
    private readonly ILibraryOperationService _service;
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ResponseModel>> _dict = new();

    public RequestClient(ILibraryOperationService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    public async Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var tcs = new TaskCompletionSource<ResponseModel>();

        if (_dict.TryAdd(id, tcs))
        {
            CancellationTokenRegistration registry = cancellationToken.Register(() => RefuseOperation(id));
            try
            {
                _service.BeginOperation(id, request, cancellationToken);
                return await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                await registry.DisposeAsync();
                _dict.TryRemove(id, out _);
            }
        }

        throw new InvalidOperationException($"Invalid request, guid {id} match found");
    }

    public void HandleOperationResult(Guid requestId, byte[] data)
    {
        if (!_dict.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? tcs))
        {
            return;
        }

        tcs.SetResult(new ResponseModel(data));
    }

    public void HandleOperationError(Guid requestId, Exception exception)
    {
        if (!_dict.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? tcs))
        {
            return;
        }

        tcs.SetException(exception);
    }

    public void RefuseOperation(Guid id)
    {
        _dict.GetValueOrDefault(id)?.TrySetCanceled();
    }
}