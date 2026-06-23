using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class ProductService : IProductService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandAsync(CancellationToken ct = default)
        {
            var brands = await unitOfWork.GetRepository<ProductBrand,int>().GetAllAsync(ct);
            var data = mapper.Map<IReadOnlyList<BrandDto>>(brands);
            return Result<IReadOnlyList<BrandDto>>.Ok(data);
        }

        public async Task<Result<IReadOnlyList<ProductDto>>> GetAllProductsAsync(CancellationToken ct = default)
        {
            var products = await unitOfWork.GetRepository<Product,int>().GetAllAsync(ct);
            return Result<IReadOnlyList<ProductDto>>.Ok(mapper.Map<IReadOnlyList<ProductDto>>(products));
        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken ct = default)
        {
            var Types = mapper.Map<IReadOnlyList<TypeDto>>(await unitOfWork.GetRepository<ProductType,int>().GetAllAsync(ct));
            return Result<IReadOnlyList<TypeDto>>.Ok(Types);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await unitOfWork.GetRepository<Product,int>().GetByIdAsync(id, ct);
            if (product == null)
            {
                return Result<ProductDto>.Fail(Error.NotFound("Product.NotFound", $"Product with id {id} is not found"));
            }
            return Result<ProductDto>.Ok(mapper.Map<ProductDto>(product));
        }
    }
}
