using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Middleware
{
    public class GrayLogService
    {
        private readonly IConfiguration configuration;

        public GrayLogService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void SaveGraylogAsync(dynamic json)
        {
            var baseUrl = configuration["URL_GRAYLOG"];
            var token = configuration["TOKEN_GRAYLOG"];

            var request = new RestRequest();
            request.AddHeader("Authorization", $"Basic {token}");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(json);

            var client = new RestClient(baseUrl);
            client.Post(request);
        }
    }
}
