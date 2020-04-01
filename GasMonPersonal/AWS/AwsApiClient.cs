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
    public class AwsApiClient
    {
        private static readonly RegionEndpoint Region = RegionEndpoint.EUWest2;

        private const string SnsTopicArn =
            "arn:aws:sns:eu-west-2:099421490492:GasMonitoring-snsTopicSensorDataPart1-1YOM46HA51FB";

        private const string BucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";

        private const string LocationFileName = "locations.json";

        private const string QueueName = "SmjGasMonQueue";

        private readonly BasicAWSCredentials _credentials;

        public AwsApiClient()
        {
            _credentials = new BasicAWSCredentials(
                Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID"),
                Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")
            );
        }

        public async Task<string> FetchLocationFileAsString()
        {
            var getLocationsRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = LocationFileName
            };

            using var client = new AmazonS3Client(_credentials);

            var locationResponse = await client.GetObjectAsync(getLocationsRequest);

            await using var responseStream = locationResponse.ResponseStream;

            return new StreamReader(responseStream).ReadToEnd();
        }

        public async Task<string> CreateQueueAndReturnUrl()
        {
            using var sqs = new AmazonSQSClient(_credentials);
            var request = new CreateQueueRequest(QueueName);
            var response = await sqs.CreateQueueAsync(request);
            return response.QueueUrl;
        }

        public async Task<string> SubscribeQueueToGasNotificationTopic(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(_credentials);
            using var sns = new AmazonSimpleNotificationServiceClient(_credentials);

            return await sns.SubscribeQueueAsync(SnsTopicArn, sqs, queueUrl);
        }

        public async Task<IEnumerable<string>> PopNextQueueMessages(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(_credentials);

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

        public async Task UnsubscribeQueue(string subscriptionArn)
        {
            using var sqs = new AmazonSQSClient(_credentials);
            using var sns = new AmazonSimpleNotificationServiceClient(_credentials);

            await sns.UnsubscribeAsync(subscriptionArn);
        }

        public async Task DeleteQueue(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(_credentials);

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

        // TODO: ensure idempotent
    }
}