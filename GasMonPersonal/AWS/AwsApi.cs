using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GasMonPersonal.Models;

namespace GasMonPersonal.AWS
{
    public class AwsApi
    {
        private static string bucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";

        private static string locationFileName = "locations.json";
        
        private readonly LocationReader _locationReader;

        public AwsApi(LocationReader locationReader)
        {
            this._locationReader = locationReader;
        }

        public async Task<IEnumerable<Location>> FetchLocationsFromS3()
        {
            return this._locationReader.Read(await FetchLocationFile());
        }

        private async Task<string> FetchLocationFile()
        {
            var client = new HttpClient();

            var locationResponse = await client.GetAsync($"S3://{bucketName}/{locationFileName}");

            return await locationResponse.Content.ReadAsStringAsync();
        }
    }
}