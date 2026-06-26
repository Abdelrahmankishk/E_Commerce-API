using E_Commerce.Application.Common;
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
        public ProductWithTypeAndBrandSpec(ProductQueryParams queryParams) : base(p => (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId.Value) && (!queryParams.TypeId.HasValue || p.TypeId == queryParams.TypeId.Value) && (string.IsNullOrWhiteSpace(queryParams.SearchValue) || p.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))
        {
            AddIncludeExp(P => P.ProductType);
            AddIncludeExp(P => P.ProductBrand);

            switch (queryParams.Sort)
            {
                case ProductSort.NameAscending:
                    AddOrderBy(P => P.Name);
                        break;
                case ProductSort.NameDescending:
                    AddOrderByDesc(P => P.Name);
                    break;
                case ProductSort.PriceAscending:
                    AddOrderBy(P => P.Price);
                    break;
                case ProductSort.PriceDescending:
                    AddOrderByDesc(P => P.Price);
                    break;
                default:
                    AddOrderBy(P => P.Id);
                    break;
            }
        }

        //Get Product by ID
        public ProductWithTypeAndBrandSpec(int id): base(x=> x.Id == id)
        {
            AddIncludeExp(P => P.ProductType);
            AddIncludeExp(P => P.ProductBrand);
        }
    }
}
