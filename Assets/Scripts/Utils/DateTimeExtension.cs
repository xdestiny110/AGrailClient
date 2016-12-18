using System;

public static class DateTimeExtension
{
    public static DateTime dt1970 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

    public static long GetMiliSecFrom1970(this DateTime dt)
    {
        return (long)dt.Subtract(dt1970).TotalMilliseconds;
    }

    public static DateTime GetDateTime(this long miliSec)
    {
        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        TimeSpan toNow = new TimeSpan(miliSec * 10000);
        return dt.Add(toNow);
    }
}
