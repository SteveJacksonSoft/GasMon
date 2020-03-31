using System.Collections.Generic;
using System.Configuration;
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

        private const string QueueName = "GasMonQueue";
        
        private readonly BasicAWSCredentials _credentials;

        public AwsApiClient()
        {
            var credentials = ConfigurationManager.GetSection("AwsCredentials") as AwsCredentials;
            this._credentials = new BasicAWSCredentials(credentials.AccessKey, credentials.SecretKey);
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

        public async Task SubscribeQueueToGasNotificationTopic(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(_credentials);
            using var sns = new AmazonSimpleNotificationServiceClient(_credentials);

            await sns.SubscribeQueueAsync(SnsTopicArn, sqs, queueUrl);
        }

        public async Task<IEnumerable<string>> PopNextQueueMessages(string queueUrl)
        {
            using var sqs = new AmazonSQSClient(_credentials);
            
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                WaitTimeSeconds = 20
            };

            var response = await sqs.ReceiveMessageAsync(request);

            DeleteSqsMessages(sqs, response.Messages);
            
            return response.Messages.Select(message => message.Body);
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
                await sqs.DeleteMessageAsync(new DeleteMessageRequest {
                    ReceiptHandle = message.ReceiptHandle
                });
            }
        }
        
        // TODO: ensure idempotent
    }
}