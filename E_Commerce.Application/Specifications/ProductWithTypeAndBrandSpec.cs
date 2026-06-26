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
        //Get All
        public ProductWithTypeAndBrandSpec(int? BrandId, int? TypeID) : base(p => (!BrandId.HasValue || p.BrandId == BrandId.Value) && (!TypeID.HasValue || p.TypeId == TypeID.Value))
        {
            AddIncludeExp(P => P.ProductType);
            AddIncludeExp(P => P.ProductBrand);
        }

        //Get Product by ID
        public ProductWithTypeAndBrandSpec(int id): base(x=> x.Id == id)
        {
            AddIncludeExp(P => P.ProductType);
            AddIncludeExp(P => P.ProductBrand);
        }
    }
}
