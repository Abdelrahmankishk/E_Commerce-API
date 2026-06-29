using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class ProductQueryParams
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 10;
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? SearchValue { get; set; }
        public ProductSort Sort { get; set; }
        private int pageSize = DefaultPageSize;

        public int PageSize {
            get
            {
                return pageSize;
            }
            set
            {
                if(value > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }
                else if (value < 1)
                {
                    pageSize = DefaultPageSize;
                }
                else
                {
                    pageSize = value;
                }
            }
        }
        public int PageIndex { get; set; } = 1;
    }
}
