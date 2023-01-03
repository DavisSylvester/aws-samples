using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DavisSylvester
{
    public class HelloWorld
    {

        public APIGatewayHttpApiV2ProxyResponse Run(APIGatewayHttpApiV2ProxyRequest apigProxyEvent,
     ILambdaContext context)
        {
            context.Logger.LogLine($"Received {apigProxyEvent}");

            return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = JsonSerializer.Serialize(new
                {
                    Message = "Hello World"
                }),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}