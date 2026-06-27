using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Basketd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IBasketService
    {
        // Get Basket => Take BasketId , Return Basket Dto
        Task<Result<BasketDto>> GetBasketAsync(string BasketId, CancellationToken ct = default);

        // Create Or update Basket => Take Basket , Return Basket After Creation Or Update
        Task<Result<BasketDto>> CreateOrUpdateBasketAsync(BasketDto basket, TimeSpan? TimeToStay = default, CancellationToken ct = default);

        // Delete Basket -> Basket Id -> Bool
        Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default);
    }
}
