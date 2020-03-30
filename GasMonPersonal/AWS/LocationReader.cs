using System.Collections.Generic;
using System.Linq;
using GasMonPersonal.Models;
using Json.Net;

namespace GasMonPersonal.AWS
{
    public class LocationReader
    {
        public IEnumerable<Location> Read(string locationsFile)
        {
            return JsonNet.Deserialize<List<string[]>>(locationsFile)
                .Select(
                    array => new Location
                    {
                        Uuid = array[0],
                        x = double.Parse(array[1]),
                        y = double.Parse(array[2])
                    }
                );
        }
    }
}