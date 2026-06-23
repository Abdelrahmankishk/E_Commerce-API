using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }
        //Get All Products
        [HttpGet]
        public async Task<ActionResult<Result<IReadOnlyList<ProductDto>>>> GetAllProducts(CancellationToken ct)
        {
            var result = await productService.GetAllProductsAsync(ct);
            return Ok(result);
        }
        //Get Product By Id
        [HttpGet("{id}")]
        public ActionResult<Result<ProductDto>> GetProductById(int id, CancellationToken ct)
        {
            var result = productService.GetProductByIdAsync(id, ct);
            return Ok(result);
        }
        //Get All Types
        [HttpGet("types")]
        public async Task<ActionResult<Result<IReadOnlyList<TypeDto>>>> GetAllTypes(CancellationToken ct)
        {
            return Ok(await productService.GetAllTypesAsync(ct));
        }
        //Get All Brands
        [HttpGet("brands")]
        public async Task<ActionResult<Result<IReadOnlyList<BrandDto>>>> GetAllBrands(CancellationToken ct)
        {
            return Ok(await productService.GetAllBrandAsync(ct));
        }
    }
}
