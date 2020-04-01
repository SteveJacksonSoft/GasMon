using System;

namespace GasMonPersonal.DateTimeConversions
{
    public static class DateTimeConverter
    {
        public static DateTime ToDateTime(this long unixEpochInSeconds)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixEpochInSeconds).DateTime;
        }
    }
}