using System;
using System.Diagnostics;
using System.Globalization;

namespace Falcon.FalconCore.Scripts.Utils {
    public static class FalconTimeUtils {

        public static long CurrentTimeMillis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public static long CurrentTimeSec()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        public static long NanoTime() {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        public static String DateToString(DateTime dateTime)
        {
            return Convert.ToString(dateTime.Year, CultureInfo.InvariantCulture)
                   + "-" + (dateTime.Month < 10
                       ? "0" + Convert.ToString(dateTime.Month, CultureInfo.InvariantCulture)
                       : Convert.ToString(dateTime.Month, CultureInfo.InvariantCulture))
                   + "-" + (dateTime.Day < 10
                       ? "0" + Convert.ToString(dateTime.Day, CultureInfo.InvariantCulture) 
                       : Convert.ToString(dateTime.Day, CultureInfo.InvariantCulture));
        }
    }
}