using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Orders;

namespace E_Commerce.Application.Contracts;

public interface IOrderService
{
    Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default);
}
