using EfCorePractice.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace EfCorePractice.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "请求处理失败");
            await WriteErrorAsync(context, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (status, message) = ex switch
        {
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "并发冲突，请刷新后重试"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "服务器内部错误")
        };

        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new ApiError(message, ex.GetType().Name));
    }
}
