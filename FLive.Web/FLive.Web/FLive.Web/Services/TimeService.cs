using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FLive.Web.Data;
using Microsoft.EntityFrameworkCore;
using FLive.Web.Models;
using FLive.Web.Models.ApplicationViewModels;


namespace FLive.Web.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    //public class TimeService 
    //{
    //    public Task SendEmailAsync(string email, string subject, string message)
    //    {
    //        // Plug in your email service here to send an email.
    //        return Task.FromResult(0);
    //    }
    //    public  async Task<DateTime> GetLocalDateTime(double latitude, double longitude, DateTime utcDate)
    //    {
    //        var client = new RestClient("https://maps.googleapis.com");
    //        var request = new RestRequest("maps/api/timezone/json", Method.GET);
    //        request.AddParameter("location", latitude + "," + longitude);
    //        request.AddParameter("timestamp", utcDate.ToTimestamp());
    //        request.AddParameter("sensor", "false");
    //        var response= client.exe

    //        return utcDate.AddSeconds(response.Data.rawOffset + response.Data.dstOffset);
    //    }
    //}

    public static class ExtensionMethods
    {
        public static double ToTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static int AsInt(this string source)
        {
            return Int32.Parse(source);
        }

        public static double? AsNullableDouble(this string source)
        {
            try
            {
                return Double.Parse(source);
            }
            catch
            {
                return null;
            }
        }

        public static int? AsNullableInt(this string source)
        {
            try
            {
                return Int32.Parse(source);
            }
            catch
            {
                return null;
            }
        }
    }

}
