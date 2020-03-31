using System.Collections.Generic;
using System.Threading.Tasks;
using GasMonPersonal.AWS;
using GasMonPersonal.Models;

namespace GasMonPersonal.Locations
{
    public class LocationFetching
    {
        public static async Task<IEnumerable<Location>> FetchLocations(AwsApiClient apiClient)
        {
            return new LocationReader().Read(await apiClient.FetchLocationFileAsString());
        }
    }
}