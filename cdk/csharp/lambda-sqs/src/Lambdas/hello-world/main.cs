using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DavisSylvester
{
    public class HelloWorld
    {

        public async Task<APIGatewayHttpApiV2ProxyResponse> Run(APIGatewayHttpApiV2ProxyRequest apigProxyEvent,
     ILambdaContext context)
        {
            context.Logger.LogLine($"Received {apigProxyEvent}");

            context.Logger.LogInformation($"CONTEXT EVENT:  {context}");

            var msg  = JsonSerializer.Serialize(new
                {
                    Message = context
                });

            var result = await sendMessage(msg);

            return result;
        }

        private async Task<APIGatewayHttpApiV2ProxyResponse> sendMessage(string message)
        {
            
            string queueUrl = $"https://sqs.us-east-1.amazonaws.com/313327262346/test-davis-two";

            IAmazonSQS client = new AmazonSQSClient();

            var request = new SendMessageRequest
            {
                MessageBody = message,
                QueueUrl = queueUrl,
                
            };

            var response = await client.SendMessageAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {  
                return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = JsonSerializer.Serialize(new
                {
                    Message = $"Successfully sent message. Message ID: {response.MessageId}"
                }),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            }
            else
            {
                // Console.WriteLine("Could not send message.");

                return new APIGatewayHttpApiV2ProxyResponse
            {
                Body = JsonSerializer.Serialize(new
                {
                    Message = "Could not send message."
                }),
                StatusCode = 400,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
            }
        }
    }


}