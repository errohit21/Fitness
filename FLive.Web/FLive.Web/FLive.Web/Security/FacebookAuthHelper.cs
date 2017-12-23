// Copyright (c) Nate Barbettini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FLive.Web.Shared;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;
using Newtonsoft.Json;
using SimpleTokenProvider;

namespace FLive.Web.Security
{
    
    public class FacebookAuthHelper
    {

        public static async Task<FacebookMeResponse> VerifyAccessToken(string email, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new BusinessException("Invalid Token");
            }

            string facebookGraphUrl = "https://graph.facebook.com/me?fields=cover,age_range,first_name,location,last_name,hometown,gender,birthday,email&access_token=" + accessToken;
            WebRequest request = WebRequest.Create(facebookGraphUrl);
            request.Credentials = CredentialCache.DefaultCredentials;

            using (WebResponse response = await request.GetResponseAsync())
            {
                var status = ((HttpWebResponse)response).StatusCode;

                Stream dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                var facebookUser = JsonConvert.DeserializeObject<FacebookMeResponse>(responseFromServer);

                bool valid = facebookUser != null && !string.IsNullOrWhiteSpace(facebookUser.Email) && facebookUser.Email.ToLower() == email.ToLower();
                facebookUser.PublicProfilePhotoUrl = "http://graph.facebook.com/" + facebookUser.Id + "/picture";

                if (!valid)
                {
                    throw new BusinessException("Invalid Facebook token");
                }

                return facebookUser;
            }
        }
    }

    public class FacebookAgeRange
    {
        public int Min { get; set; }
    }

    public class FacebookMeResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public FacebookAgeRange AgeRange { get; set; }
        public string PublicProfilePhotoUrl { get; set; }
    }

    public class FacebookResponse 
    {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public string JwtToken { get; set; }

        public FacebookResponse(string message, bool ok = true, string jwtToken = "")
        {
            this.Message = message;
            this.Ok = ok;
            this.JwtToken = jwtToken;
        }
    }
}