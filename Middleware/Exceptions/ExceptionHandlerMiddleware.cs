using Domain.Validator;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var isDomainException = ex is DomainValidator;

                var statusCode = isDomainException ? 400 : 500;

                var message = isDomainException ? ex.Message : "Erro interno, contate o suporte";

                var _ = JsonConvert.SerializeObject
                (
                    new
                    {
                        Status = statusCode,
                        Message = message,
                        Date = DateTimeOffset.Now
                    }
                );

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(_);
            }
        }
    }
}
