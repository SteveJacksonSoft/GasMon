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
            return JsonNet.Deserialize<List<Location>>(locationsFile);
        }
    }
}