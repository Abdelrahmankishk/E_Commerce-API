using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Basketd;

namespace E_Commerce.Application.Contracts;

public interface IPaymentService
{
    Task<Result<BasketDto>> CreateOrUpdatePaymentIntentAsync(string BasketId, CancellationToken ct = default!);
}
