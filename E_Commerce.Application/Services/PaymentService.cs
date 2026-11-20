using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Basketd;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;

namespace E_Commerce.Application.Services;

internal class PaymentService : IPaymentService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IMapper _mapper;
    private readonly PaymentGatwaySettings _paymentGatwaySettings;
    public PaymentService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IPaymentGateway paymentGateway,IOptions<PaymentGatwaySettings> options,IMapper mapper)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _mapper = mapper;
        _paymentGatwaySettings = options.Value;
    }
    public async Task<Result<BasketDto>> CreateOrUpdatePaymentIntentAsync(string BasketId, CancellationToken ct = default)
    {
        var basket = await _basketRepository.GetBasketAsync(BasketId, ct);
        if (basket == null)
            return Error.NotFound("Basket Is Not Found", $"Basket with Id {BasketId} Is Not Found");

        if (basket.Items.Count == 0)
            return Error.Validation("Basket is Empty");

        if (!basket.DeliveyMethodId.HasValue)
            return Error.Validation("Delivery Method Id Is Required");

        var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveyMethodId.Value,ct);
        if (deliveryMethod == null)
            return Error.NotFound("Delivery Method Not Found");

        basket.ShippingPrice = deliveryMethod.Cost;


        var productsIds = basket.Items.Select(x => x.Id).ToHashSet();
        var products = (await _unitOfWork.GetRepository<Product, int>()
            .GetAllAsync(new ProductWithIdSpecifications(productsIds), ct)).ToDictionary(x => x.Id);

        foreach (var item in basket.Items)
        {
            if (!products.TryGetValue(item.Id, out var product))
                return Error.NotFound("Product Not Found");

            item.Price = product.Price;
        }

        var subTotal = basket.Items.Sum(i => i.Price * i.Quantity);
        var amount = (long)((subTotal + deliveryMethod.Cost) * 100m);

        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {

            var result = await _paymentGateway.CreatePaymentIntentAsync(amount, _paymentGatwaySettings.DefaultCurrency, ct);
            basket.PaymentIntentId = result.PaymentIntntId;
            basket.ClientSecret = result.ClientSecret;
        }
        else
        {
            await _paymentGateway.UpdatePaymentIntentAsync(amount, basket.PaymentIntentId, ct);
        }

        await _basketRepository.CreateOrUpdateBasketAsync(basket, ct: ct);

        return _mapper.Map<BasketDto>(basket);
    }
}
