using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IO;

namespace Middleware
{
    public class GrayLogHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly GrayLogService grayLogService;
        private readonly bool isGrayLogEnabled;
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        private readonly string applicationName = "MARKETPLACE";

        public GrayLogHandlerMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            grayLogService = new GrayLogService(configuration);
            isGrayLogEnabled = Convert.ToBoolean(configuration["ENABLE_GRAYLOG"]);
            recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            if (isGrayLogEnabled)
            {
                await LogRequest(context);
                await LogResponse(context);
            }
            else
                await next(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = recyclableMemoryStreamManager.GetStream();

            await context.Request.Body.CopyToAsync(requestStream);

            grayLogService.SaveGraylogAsync(new
            {
                type = "request",
                application_name = applicationName,
                short_message = applicationName,
                method = context.Request.Method,
                url = context.Request.GetDisplayUrl(),
                level = 1,
                request = ReadStreamInChunks(requestStream)
            });

            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            grayLogService.SaveGraylogAsync(new
            {
                type = "response",
                application_name = applicationName,
                short_message = applicationName,
                method = context.Request.Method,
                url = context.Request.GetDisplayUrl(),
                level = 1,
                response = text,
                statusCode = context.Response.StatusCode
            });

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }

}
