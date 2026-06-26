using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithTypeAndBrandSpec : BaseSpecification<Product,int>
    {
        public ProductWithTypeAndBrandSpec()
        {
            AddIncludeExp(P => P.ProductType);
            AddIncludeExp(P => P.ProductBrand);
        }
    }
}
