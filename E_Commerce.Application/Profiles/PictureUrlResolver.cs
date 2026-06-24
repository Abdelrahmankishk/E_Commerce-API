using AutoMapper;
using AutoMapper.Execution;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    internal class PictureUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly Urlsetting urlsetting;

        public PictureUrlResolver(IOptions<Urlsetting> options)
        {
            urlsetting = options.Value;
        }
        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            var BaseUrl = urlsetting.BaseUrl.TrimEnd('/');
            var path = source.PictureUrl.TrimEnd('/');
            return $"{BaseUrl}/Files/{path}";
        }
    }

    public class Urlsetting
    {
        public string BaseUrl { get; set; }
    }
}
