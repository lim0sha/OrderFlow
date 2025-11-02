namespace Gateway.Models.DTO.Orders;

public record GetHistoryRequestDto(int Cursor, int Volume, FilterDto Filter);