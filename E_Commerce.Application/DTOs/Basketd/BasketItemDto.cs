using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Application.DTOs.Basketd
{
    public class BasketItemDto
    {
        [Required(ErrorMessage = "Product ID is Reduired")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Product Name is Reduired")]
        public string ProductName { get; set; } = default!;
        public string ProductUrl { get; set; } = default!;
        [Range(1,double.MaxValue)]
        public decimal Price { get; set; }
        [Range(1,50)]
        public int Quantity { get; set; }
    }
}