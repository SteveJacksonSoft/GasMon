using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace GasMonPersonal.AWS
{
    public class AwsApiClient
    {
        private static RegionEndpoint region = RegionEndpoint.EUWest2;
        
        private static string snsTopicArn =
            "arn:aws:sns:eu-west-2:099421490492:GasMonitoring-snsTopicSensorDataPart1-1YOM46HA51FB";
        
        private static string bucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";

        private static string locationFileName = "locations.json";
        
        private BasicAWSCredentials _credentials;

        public AwsApiClient()
        {
            var credentials = ConfigurationManager.GetSection("AwsCredentials") as AwsCredentials;
            this._credentials = new BasicAWSCredentials(credentials.AccessKey, credentials.SecretKey);
        }

        public async Task<string> FetchLocationFileAsString()
        {
            var getLocationsRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = locationFileName
            };
            
            using var client = new AmazonS3Client(_credentials);

            var locationResponse = await client.GetObjectAsync(getLocationsRequest);

            await using var responseStream = locationResponse.ResponseStream;

            return new StreamReader(responseStream).ReadToEnd();
        }

        public async Task CreateQueue()
        {
            var sqs = new AmazonSQSClient(_credentials);
            var request = new CreateQueueRequest();
            sqs.CreateQueueAsync("");
        }
    }
}