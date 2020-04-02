using System;

namespace GasMonPersonal.DateTimeConversion
{
    public static class DateTimeConverter
    {
        public static DateTime ToDateTime(this long unixEpochInSeconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixEpochInSeconds).DateTime;
        }
    }
}