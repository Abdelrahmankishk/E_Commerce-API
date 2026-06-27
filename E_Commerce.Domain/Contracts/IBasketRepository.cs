using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    internal interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasketAsync(string id, CancellationToken ct = default);
        Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? TimeToStay = default, CancellationToken ct = default);
        Task<bool> DeleteBasketAsync(string id, CancellationToken ct = default);
    }
}
