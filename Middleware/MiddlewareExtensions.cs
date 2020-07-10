using Microsoft.AspNetCore.Builder;

namespace Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGrayLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GrayLogHandlerMiddleware>();
        }

        public static IApplicationBuilder UseCustomException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}