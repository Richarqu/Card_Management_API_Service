using CardMGTService.Core.Domain.Model;
using CardMGTService.Core.Dtos;
using CardMGTService.Core.Persistence.EF;
using CardMGTService.Core.Repositories;
using CardMGTService.Core.Services.Impl;
using CardMGTService.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace CardMGTService.API.Extension
{

    public class CardAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly NLog.Logger Logger = NLog.LogManager.GetLogger(nameof(CardAuthorizeAttribute));
        
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (IsAuthorized(actionContext))
                return;

            VirtualCardResponse rsp = new VirtualCardResponse
            {
                status = "failed",
                ResponseCode = "-1",
                ResponseMessage = "Invalid client credentials"
            };

            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, rsp);
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var _uow = new UnitOfWork(new CardMGTContext());
            _uow.Log.Add(new Log() {Type="Info", Description= $"Request got here from client with request details {actionContext.Request}", DateLogged = DateTime.Now });
            _uow.Complete();


            #region HMAC256 Hashing

            var request = actionContext.Request;

            var headers = request.Headers;
            IEnumerable<string> checksum = new List<string>();
            IEnumerable<string> clientId = new List<string>();
            if (!headers.TryGetValues("X-CS", out checksum) || !headers.TryGetValues("X-CID", out clientId))
                return false;

            _uow.Log.Add(new Log() { Type = "Info", Description = $"Id of Requesting Client is {clientId.First()}", DateLogged = DateTime.Now });

            _uow.Complete();

            var client =_uow.ApiClient.GetClientByClientIdAsync(clientId.First()).GetAwaiter().GetResult();

            if(client == null || client.ClientSecret == null)
            {
                VirtualCardResponse rsp = new VirtualCardResponse
                {
                    status="failed",
                    ResponseCode = "-1",
                    ResponseMessage = "Invalid Client Credentials"
                };

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, rsp);
                return false;
            }

            string key = client.ClientSecret;//Note use clientid to fetch this from the Db;

            var content = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            return ValidateData(content, key, checksum.First());

            #endregion
        }


        bool ValidateData(string payload, string key, string hashToCompare)
        {
            var hashedModel = Hashing.HmacSHA256(payload, Convert.FromBase64String(key));

            return string.Equals(hashedModel, hashToCompare);
        }
    }


}