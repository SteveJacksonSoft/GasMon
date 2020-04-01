using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace GasMonPersonal.AWS
{
    public static class AwsService
    {
        private static readonly RegionEndpoint Region = RegionEndpoint.EUWest2;

        private const string SnsTopicArn =
            "arn:aws:sns:eu-west-2:099421490492:GasMonitoring-snsTopicSensorDataPart1-1YOM46HA51FB";

        private const string BucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";

        private const string LocationFileName = "locations.json";

        private const string QueueName = "SmjGasMonQueue";

        private static  readonly BasicAWSCredentials Credentials = new BasicAWSCredentials(
            Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
            Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
        );

        public static async Task<string> FetchLocationFileAsString()
        {
            var getLocationsRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = LocationFileName
            };

            using var client = new AmazonS3Client(Credentials);

            var locationResponse = await client.GetObjectAsync(getLocationsRequest);

            await using var responseStream = locationResponse.ResponseStream;

            return new StreamReader(responseStream).ReadToEnd();
        }

        public static async Task<string> CreateQueueAndReturnUrl()
        {
            using var sqs = new AmazonSQSClient(Credentials);
            var request = new CreateQueueRequest(QueueName);
            var response = await sqs.CreateQueueAsync(request);
            return response.QueueUrl;
        }

        public static async Task<string> SubscribeQueueToGasNotificationTopic(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(Credentials);
            using var sns = new AmazonSimpleNotificationServiceClient(Credentials);

            return await sns.SubscribeQueueAsync(SnsTopicArn, sqs, queueUrl);
        }

        public static async Task<IEnumerable<string>> PopNextQueueMessages(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(Credentials);

            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                WaitTimeSeconds = 20,
                MaxNumberOfMessages = 10,
            };

            var response = await sqs.ReceiveMessageAsync(request);

            DeleteSqsMessages(sqs, response.Messages);

            return response.Messages.Select(message => message.Body);
        }

        public static async Task UnsubscribeQueue(string subscriptionArn)
        {
            using var sqs = new AmazonSQSClient(Credentials);
            using var sns = new AmazonSimpleNotificationServiceClient(Credentials);

            await sns.UnsubscribeAsync(subscriptionArn);
        }

        public static async Task DeleteQueue(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(Credentials);

            await sqs.DeleteQueueAsync(queueUrl);
        }

        private static async Task DeleteSqsMessages(IAmazonSQS sqs, IEnumerable<Message> messages)
        {
            foreach (var message in messages)
            {
                await sqs.DeleteMessageAsync(
                    new DeleteMessageRequest
                    {
                        ReceiptHandle = message.ReceiptHandle
                    }
                );
            }
        }
    }
}