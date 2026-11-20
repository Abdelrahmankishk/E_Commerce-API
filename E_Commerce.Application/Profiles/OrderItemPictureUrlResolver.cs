using AutoMapper;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Domain.Entities.Orders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles;

internal class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
{
    private readonly Urlsetting _setting;
    public OrderItemPictureUrlResolver(IOptions<Urlsetting> options)
    {
        _setting = options.Value;
    }
    public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
    {
        var baseUrl = _setting.BaseUrl.TrimEnd('/');
        var path = source.Product.PictureUrl.TrimStart('/');
        return $"{baseUrl}/Files/{path}";
    }
}
