using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;

namespace E_Commerce.Application.Services;

internal class OrderService : IOrderService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
    {
        var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId, ct);
        if (basket == null)
            return Error.NotFound("Basket Not Found", $"Basket With Id {orderDto.BasketId} Is Not Found");

        if (basket.Items.Count == 0)
            return Error.Validation("Basket Is Empty", $"Can not Create Order With Basket Id {basket.Id}");

        // Items (Order Item)
        var orderItems = new List<OrderItem>(basket.Items.Count);
        var productIds = basket.Items.Select(x => x.Id).ToHashSet();
        var products = (await _unitOfWork.GetRepository<Product, int>()
                                        .GetAllAsync(new ProductWithIdSpecifications(productIds), ct)).ToDictionary(x => x.Id);

        foreach (var item in basket.Items)
        {
            //Get Product

            if (!products.TryGetValue(item.Id, out var product))
                return Error.NotFound("Product Not Found", $"Product With Id {item.Id} Is Not Found");

            orderItems.Add(new OrderItem()
            {
                Price = product.Price,
                Quantity = item.Quantity,
                Product = new ProductItemOrdered()
                {
                    PictureUrl = product.PictureUrl,
                    ProductId = product.Id,
                    ProductName = product.Name,
                }

            });


        }

        //ShipToAddress(OrderAddress)

        var orderAddress = _mapper.Map<OrderAddress>(orderDto.ShipToAddress);

        //Delivery Method
        var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(orderDto.DeliveryMethodId);

        if (deliveryMethod is null)
            return Error.NotFound("Delivery Method Not Found", $"Delivery Method With Id {orderDto.DeliveryMethodId} Is Not Found");

        //SubTotal
        var subTotal = orderItems.Sum(x => x.Quantity * x.Price);



        //Create Order
        var order = new Order(email, orderAddress, orderItems, deliveryMethod, subTotal);

        _unitOfWork.GetRepository<Order, Guid>().Add(order);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        if (result == 0)
        {
            return Error.Failure("Order Save Fail", $"Can Not Create Order");
        }
        else
        {
            await _basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);
            return _mapper.Map<OrderToReturnDto>(order);
        }
    }
}
