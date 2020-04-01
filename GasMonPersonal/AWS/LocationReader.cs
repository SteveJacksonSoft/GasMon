using System.Collections.Generic;
using GasMonPersonal.Models;
using Newtonsoft.Json;

namespace GasMonPersonal.AWS
{
    public class LocationReader
    {
        public IEnumerable<Location> Read(string locationsFile)
        {
            return JsonConvert.DeserializeObject<List<Location>>(locationsFile);
        }
    }
}