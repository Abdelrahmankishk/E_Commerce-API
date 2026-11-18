using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace E_Commerce.API.Attributes;

public class RedisCacheAttribute : ActionFilterAttribute
{
    private readonly int _durationInSeconds;

    public RedisCacheAttribute(int durationInSeconds = 60)
    {
        _durationInSeconds = durationInSeconds;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //Get Cache Service from DI Container
        var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        var cacheKey = CreateCacheKey(context.HttpContext.Request);

        var data = await cacheService.GetDataAsync(cacheKey);

        //If Data is in Cache, return it

        if (!string.IsNullOrEmpty(data))
        {
            context.Result = new ContentResult
            {
                Content = data,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
            };
            return;
        }

        //If Data is not in Cache, execute the action and cache the result
        var executedContext = await next.Invoke();
        if (executedContext.Result is OkObjectResult { Value: not null } ok)
        {
            await cacheService.SetDataAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(_durationInSeconds));
        }

    }

    private static string CreateCacheKey(HttpRequest request)
    {
        var key = new StringBuilder();
        key.Append(request.Path);
        if (request.Query.Any())
        {
            key.Append('?');
            foreach (var (k, v) in request.Query.OrderBy(x => x.Key))
            {
                key.Append($"{k}={v}&");
            }
        }
        return key.ToString();

    }
}
