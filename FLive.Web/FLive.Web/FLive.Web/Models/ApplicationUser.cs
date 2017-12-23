using System;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Internal.Networking;
using Newtonsoft.Json;

namespace FLive.Web.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
   
    public class ApplicationUser : IdentityUser
    {
        public UserType UserType { get; set; }
        public string Timezone { get; set; }
        public string DeviceToken { get; set; }
        public string Platform { get; set; }
        public string Name { get; set; }
        //public int Age { get; set; }
        public string Age { get; set; }
        public string ProfileImageUrl { get; set; }

        public string FacebookToken { get; set; }
        public string PostCode { get; set; }
        public string MobileNumber { get; set; }

		public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }


        [JsonIgnore]
        public override string PasswordHash
        {
            get { return base.PasswordHash; }
            set { base.PasswordHash = value; }
        }

        [JsonIgnore]
        public override string SecurityStamp
        {
            get { return base.SecurityStamp; }
            set { base.SecurityStamp = value; }
        }

        [JsonIgnore]
        public override string ConcurrencyStamp
        {
            get { return base.ConcurrencyStamp; }
            set { base.ConcurrencyStamp = value; }
        }

        [JsonIgnore]
        public override string NormalizedUserName
        {
            get { return base.NormalizedUserName; }
            set { base.NormalizedUserName = value; }
        }

        [JsonIgnore]
        public override string NormalizedEmail
        {
            get { return base.NormalizedEmail; }
            set { base.NormalizedEmail = value; }
        }

        [JsonIgnore]
        public override bool EmailConfirmed
        {
            get { return base.EmailConfirmed; }
            set { base.EmailConfirmed = value; }
        }

        [JsonIgnore]
        public override bool PhoneNumberConfirmed
        {
            get { return base.PhoneNumberConfirmed; }
            set { base.PhoneNumberConfirmed = value; }
        }
        [JsonIgnore]
        public override bool LockoutEnabled
        {
            get { return base.LockoutEnabled; }
            set { base.LockoutEnabled = value; }
        }

        [JsonIgnore]
        public override int AccessFailedCount 
        {
            get { return base.AccessFailedCount; }
            set { base.AccessFailedCount = value; }
        }

        [JsonIgnore]
         public override DateTimeOffset? LockoutEnd 
        {
            get { return base.LockoutEnd; }
            set { base.LockoutEnd = value; }
        }

        

    }
}
