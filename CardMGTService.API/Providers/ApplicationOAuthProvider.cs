using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CardMGTService.API.Models;
using CardMGTService.Core.Services.Interfaces;
using CardMGTService.Core.Domain.Model;
using CardMGTService.Core.Persistence.EF;
using CardMGTService.Core.Services.Impl;
using SimpleInjector;
using System.Configuration;
using CardMGTService.Core.Repositories;
using CardMGTService.Core.Persistence.Cache;
using CardMGTService.API.App_Start;

namespace CardMGTService.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly Func<IUserManagerService<User>> _userManager;
      
        public IUserManagerService<User> UserMgr { get { return _userManager.Invoke(); } }
      
      

        public ApplicationOAuthProvider(Func<IUserManagerService<User>> userManager)
        {
            _userManager = userManager;
        }

        private const string CardExclusionClientId = "X-Sterling-ADMIN";

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;
            context.TryGetFormCredentials(out clientId, out clientSecret);

            try
            {
                if (clientId != CardExclusionClientId && (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)))
                {
                    context.SetError("invalid_grant", "Non existent or invalid client.");
                    return;
                }

                if (clientId != CardExclusionClientId && !await UserMgr.ClientExistsAsync(clientId, clientSecret))
                {
                    context.SetError("invalid_grant", "Non existent or invalid client.");
                    return;
                }

                context.Validated(clientId);

                await base.ValidateClientAuthentication(context);
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", $"Internal server error validating client: {ex.Message}");
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var cacheKey = $"LOGIN-REQ-{context.UserName}";
               

                var accountLockTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["AccountLockTimeout"]);
                var maxLoginAttempts = Convert.ToInt32(ConfigurationManager.AppSettings["MaxLoginAttempts"]);

               

                if (!await UserMgr.ValidateUserCredentialsAsync(context.UserName, context.Password))
                {
                    context.SetError("invalid_grant", "The username or password is incorrect.");
                    return;
                }

              

                if (!await UserMgr.IsAccountActiveAsync(context.UserName))
                {
                    context.SetError("invalid_grant", "Your account has been blocked. Please contact admin for assistance.");
                    return;
                }

              
                var user = await UserMgr.GetUserByEmailAsync(context.UserName);
            


                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Email, context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                var profilePixResult = await UserMgr.GetProfilePixAsync(context.UserName);
                var baseUrl = Convert.ToString(ConfigurationManager.AppSettings["BaseAddress"]);
                var ProfilepixIconAddress = Convert.ToString(ConfigurationManager.AppSettings["ProfilepixIconAddress"]);

                ProfilepixIconAddress = baseUrl + ProfilepixIconAddress;
                var address = user.Address ?? "No Address";
                var phone = user.PhoneNumber ?? "No Phone";
                var profilePix = profilePixResult!=null? baseUrl + profilePixResult : ProfilepixIconAddress;
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
               
                //var user = await _userManager.GetUserByIdAsync(context.UserName);
               
          
                var userRoles = await UserMgr.GetUserRolesAsync(context.UserName);

                foreach (var role in userRoles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
               
                context.Validated(new AuthenticationTicket(identity, new AuthenticationProperties
                {
                    Dictionary = {
                        { "roles", userRoles.Count <= 1 ? userRoles.FirstOrDefault() : userRoles.Aggregate((previous, next) => $"{previous},{next}") },
                    
                        { "user_id", user.UserId.ToString() },
                        { "username", context.UserName },
                        { "firstname", user.FirstName },
                         { "phone", phone },
                          { "address", address },
                         { "lastname", user.LastName },
                          { "profilePix", profilePix  }


                    }
                }));
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", $"Internal server error validating user: {ex.Message}");
            }
        }

        private string MyDictionaryToJson(Dictionary<string, List<string>> dict)
        {
            var entries = dict.Select(d =>
                $"\"{d.Key}\": \"[{string.Join(",", d.Value)}]\"");
            return "{" + string.Join(",", entries) + "}";
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {

            foreach (var pair in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(pair.Key, pair.Value);
            }
           
            return Task.FromResult(true);
        }

        private class LoginAttempt
        {
            public int Count { get; set; }
            public DateTime DateTime { get; set; } = DateTime.Now;
        }
    }
}