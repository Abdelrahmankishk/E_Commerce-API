using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;

public class OrdersController : ApiBaseController
{
    private IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto, CancellationToken ct = default)
    {
        return ToActionResult(await _orderService.CreateOrderAsync(orderDto, GetEmailFromToken(), ct));

    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders(CancellationToken ct)
    {
        return ToActionResult(await _orderService.GetAllOrdersForUserAsync(GetEmailFromToken(), ct));

    }


    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderToReturnDto>> GetOrderById(Guid id, CancellationToken ct)
    {
        return ToActionResult(await _orderService.GetOrderByIdAndEmailForUserAsync(id, GetEmailFromToken(), ct));

    }
}
