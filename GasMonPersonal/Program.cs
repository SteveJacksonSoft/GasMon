using System;
using GasMonPersonal.AWS;

namespace GasMonPersonal
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiClient = new AwsApi(new LocationReader());
            
            Console.WriteLine();
        }
    }
}