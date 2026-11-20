using E_Commerce.Domain.Entities.Orders;

namespace E_Commerce.Application.Specifications;

internal class PaymentIntentSpec: BaseSpecification<Order,Guid>
{
    public PaymentIntentSpec(string PaymentIntentId):base(o=>o.PaymentIntentId == PaymentIntentId)
    {
        
    }
}
