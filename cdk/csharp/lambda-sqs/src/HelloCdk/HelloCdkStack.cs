using cdk = Amazon.CDK;
using SQS = Amazon.CDK.AWS.SQS;
using Constructs;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.IAM;
using System.Collections.Generic;

namespace HelloCdk
{
    public class HelloCdkStack : cdk.Stack
    {
        internal HelloCdkStack(Construct scope, string id, cdk.IStackProps props = null) : base(scope, id, props)
        {

            
            // The code that defines your stack goes here
            var testQueue = new SQS.Queue(this, "CdkLabQueue", new SQS.QueueProps
            {
                VisibilityTimeout = Duration.Seconds(300),
                QueueName = "test-davis-one.fifo",
                Fifo = true,
                DeadLetterQueue = new SQS.DeadLetterQueue()
                {
                    MaxReceiveCount = 3,
                    Queue = new SQS.Queue(this, "dlqCdkLabQueue", new SQS.QueueProps
                    {
                        QueueName = "test-davis-one-dql.fifo",
                    })
                }
            });

            

            var testQueueTwo = new SQS.Queue(this, "CdkLabQueueTwo", new SQS.QueueProps
            {
                VisibilityTimeout = Duration.Seconds(300),
                QueueName = "test-davis-two"
            });

            var helloWorldFunction = new Function(this, "HelloWorld1", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./src/Lambdas/hello-world/bin/Debug/net6.0/publish"),
                Handler = "helloWorld::DavisSylvester.HelloWorld::Run",
                Timeout = Duration.Seconds(15),
                MemorySize = 512,
                FunctionName = "Hello-World"
            }
            );

            var sendMessageFunction = new Function(this, "SendMessage", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("./src/Lambdas/send-message/bin/Debug/net6.0/publish"),
                Handler = "sendMessage::DavisSylvester.SendMessage::Run",
                Timeout = Duration.Seconds(15),
                MemorySize = 512,
                FunctionName = "Send-Message"
            }
            );

            testQueue.GrantSendMessages(sendMessageFunction);
            testQueue.GrantConsumeMessages(helloWorldFunction);

            var api = new LambdaRestApi(this, "Hello_World_APIGateway", new LambdaRestApiProps
            {
                Handler = helloWorldFunction,
                Proxy = false,
                RestApiName = "Hello_World_APIGateway"
            });

            var helloWorldLambda = new LambdaIntegration(helloWorldFunction);
            var sendMessageLambda = new LambdaIntegration(sendMessageFunction);

            api.Root.AddMethod("GET", helloWorldLambda);

            var sendEndpoint = api.Root.AddResource("send");

            sendEndpoint.AddMethod("GET", sendMessageLambda);
        }
    }
}
