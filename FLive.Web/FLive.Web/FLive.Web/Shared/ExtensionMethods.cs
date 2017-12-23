using System;
using NodaTime;

namespace FLive.Web.Shared
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object source)
        {
            return source == null;
        }
        public static bool IsNotNull(this object source)
        {
            return source != null;
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime ToUTCTime(this DateTime dateTime, string userTimezoneName)
        {

            var zone = DateTimeZoneProviders.Tzdb[userTimezoneName];
            if(zone.IsNull())
                zone = DateTimeZoneProviders.Tzdb["Australia/Melbourne"];

            var userLocalTime = dateTime.ToLocalDateTime();

            var zonedDateTime = zone.AtStrictly(userLocalTime);

            return zonedDateTime.ToDateTimeUtc();
        }

        public static LocalDateTime ToLocalDateTime(this DateTime dateTime)
        {
            return new LocalDateTime(dateTime.Year,dateTime.Month,dateTime.Day,dateTime.Hour,dateTime.Minute,dateTime.Second);
        }

    }
}