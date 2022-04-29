using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace TukTruk.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly ILogger _logger;

    public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment, ILogger logger)
    {
        _next = next;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (_webHostEnvironment.EnvironmentName == "Development")
                throw;

            var guidError = Guid.NewGuid().ToString("N");
            _logger.LogError(exception: ex, $"method:{context.Request.Method},  url: {context.Request.GetEncodedUrl()}", guidError);
            context.Response.StatusCode = HttpStatusCode.InternalServerError.GetHashCode();
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { id = $"Error: {guidError}" }));
        }
    }
}