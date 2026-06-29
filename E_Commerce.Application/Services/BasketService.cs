using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Basketd;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class BasketService : IBasketService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketService(IBasketRepository basketRepository,IMapper mapper) {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        public async Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? TimeToStay = null, CancellationToken ct = default)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);

            var basketResult = await basketRepository.CreateOrUpdateBasketAsync(customerBasket, TimeToStay, ct);

            if (basketResult == null)
            {
                return Result<BasketDto>.Fail(Error.Failure("BasketCreate.Failure", "Can Not Create Or Update Basket"));
            }else
            {
                return Result<BasketDto>.Ok(basket);
            }
        }

        public async Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default)
        {
            var result = await basketRepository.DeleteBasketAsync(basketId, ct);
            if (result)
            {
                return Result<bool>.Ok(true);
            }
            else
            {
                return Result<bool>.Fail(Error.Failure("BasketDelete.Failure", "Can Not Delete Basket"));
            }
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string BasketId, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(BasketId, ct);
            if(basket == null)
            {
                return Result<BasketDto>.Fail(Error.NotFound("Basket Not Found"));
            }
            else
            {
                return Result< BasketDto>.Ok(mapper.Map<BasketDto>(basket));
            }
        }
    }
}
