using CardMGTService.Core.Dtos;
using CardMGTService.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CardMGTService.Core.Common;
using CardMGTService.Core.Common.Enums;
using CardMGTService.Core.Domain.Model;
using CardMGTService.API.Extension;
using CardMGTService.Core.Util;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Web;
using System.Configuration;
using CardMGTService.Core.Common.Exceptions;

//using BizConnect.

namespace CardMGTService.API.Controllers
{
    [Authorize(Roles = "SterlingAdmin")]
    [RoutePrefix("Account")]
    public class AccountController : ApiControllerBase
    {
        private readonly IUserManagerService<User> _userManager;
        private readonly IServiceHelper _helper;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IMailer _messagingFactory;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IApiClientService _apiclient;
        public AccountController(IApiClientService apiclient, IPasswordGenerator passwordGenerator, IUserManagerService<User> userManager, IServiceHelper helper, IMailer messagingFactory, IPasswordHasher passwordHasher) : base(nameof(AccountController))
        {
            _userManager = userManager;
            _helper = helper;
            _passwordGenerator = passwordGenerator;
            _passwordHasher = passwordHasher;
            _messagingFactory = messagingFactory;
            _apiclient = apiclient;
        }



     
        [HttpPost]
        [Route("createuser")]
        public async Task<IApiResponse<bool>> CreateUser(UserDto registration)
        {
            return await HandleApiOperationAsync(async () =>
            {
                if (await _userManager.UserExistsAsync(registration.Email))
                {
                    throw await _helper.GetExceptionAsync("User Account already exists");
                }

                if (!await _userManager.RolesExistAsync(registration.Roles))
                {
                    throw await _helper.GetExceptionAsync("Role does not exist");
                }

               
                var user = new User
                {
                    Email = registration.Email,
                    UserName = registration.Email,
                    FirstName = registration.FirstName,
                    LastName = registration.LastName,
                    PhoneNumber = registration.PhoneNumber,
                    UserType = registration.UserType,
                    IsActive = true,

                };

                var newpassword = registration.Password;
               
                if (!await _userManager.CreateUserAsync(user, newpassword, registration.Roles))
                {
                    throw await _helper.GetExceptionAsync("User Account Registration Failed");
                }
                
                return new DefaultApiReponse<bool>();
            });
        }

        [HttpPost]
        [Route("createclient")]
        public async Task<IApiResponse<bool>> CreateClient(ApiClientDto client)
        {
            return await HandleApiOperationAsync(async () =>
            {

                await _apiclient.AddClient(client);
                return new DefaultApiReponse<bool>();
            });
        }





    }
}
