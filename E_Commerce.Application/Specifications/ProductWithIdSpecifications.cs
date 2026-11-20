using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities.Products;

namespace E_Commerce.Application.Specifications;

internal class ProductWithIdSpecifications : BaseSpecification<Product,int>
{
    public ProductWithIdSpecifications(HashSet<int> productIds) : base(P => productIds.Contains(P.Id))
    {

    }
}
