using Gateway.Models.DTO.Orders;
using Gateway.Models.Responses.Orders.AddItem;
using Gateway.Models.Responses.Orders.Cancel;
using Gateway.Models.Responses.Orders.Complete;
using Gateway.Models.Responses.Orders.Create;
using Gateway.Models.Responses.Orders.RemoveItem;
using Gateway.Models.Responses.Orders.Transfer;

namespace Gateway.Services.Interfaces;

public interface IOrderGatewayService
{
    Task<CreateOrderResponseBase> CreateOrderAsync(CreateOrderRequestDto request);

    Task<AddItemResponseBase> AddItemAsync(AddItemRequestDto request);

    Task<RemoveItemResponseBase> RemoveItemAsync(RemoveItemRequestDto request);

    Task<TransferToWorkResponseBase> TransferToWorkAsync(long orderId);

    Task<CompleteOrderResponseBase> CompleteOrderAsync(long orderId);

    Task<CancelOrderResponseBase> CancelOrderAsync(long orderId);

    Task<IEnumerable<OrderHistoryDto>> GetHistoryAsync(GetHistoryRequestDto request);
}