using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Security;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SimpleTokenProvider;

namespace FLive.Web
{
    public partial class Startup
    {
        // The secret key every token will be signed with.
        // Keep this safe on the server!
        private static readonly string secretKey = "mysupersecret_secretkey!123";

        SignInManager<ApplicationUser> _signInManager;
        UserManager<ApplicationUser> _userManager;
        private void ConfigureAuth(IApplicationBuilder app)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            
            _signInManager =  app.ApplicationServices.GetService(typeof(SignInManager<ApplicationUser>)) as SignInManager<ApplicationUser>;
            _userManager = app.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;

            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity,
                
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {

                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwtDataFormat(
                    SecurityAlgorithms.HmacSha256,
                    tokenValidationParameters)
            });

        }

        private   Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            bool signInResult = false;
            if (password == "USE_FACEBOOK_AUTH")
            {
                var registeredUser = _userManager.FindByEmailAsync(username).Result;
                signInResult = registeredUser.IsNotNull();
            }
            else
            {
               var result =  _signInManager.PasswordSignInAsync(username, password, false, false).Result;
                signInResult = result.Succeeded;
            }
               
                if (signInResult)
                {
                    var user = _userManager.FindByNameAsync(username).Result;
                    return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { new Claim("id", user.Id.ToString())  }));
                }
         

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }


    }
}