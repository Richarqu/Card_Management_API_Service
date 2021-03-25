using CardMGTService.API.Extension;
using CardMGTService.Core.Common;
using CardMGTService.Core.Dtos;
using CardMGTService.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CardMGTService.API.Controllers
{
    //[CardAuthorize]
    [RoutePrefix("CardMGT")]
    public class CardMGTController : ApiController
    {
        private readonly ICardMGTService _cardSrvc;
        private readonly ILogService _logService;

        public CardMGTController(ICardMGTService cardSrvc, ILogService logService)
        {
            _cardSrvc = cardSrvc;
            _logService = logService;
        }


        [HttpPost]
        [Route("RequestCard")]
        public async Task<VirtualCardResponse> RequestCard(VirtualCardRequest request) 
        {
           
            if (!ModelState.IsValid)
                return new VirtualCardResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside Request Virtual Card for customer with uniqueid {request.CustomerUniqueIdentifier} and email {request.EMAIL}" });

            var response = await _cardSrvc.InstanceVirtualCardReqWallet(request);
            return response;
        }

        [HttpPost]
        [Route("BlockCard")]
        public async Task<BlockCardOut> BlockCard(BlockCardIn request)
        {
          

            if (!ModelState.IsValid)
                return new BlockCardOut() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside BlockCard for customer with uniqueid {request.CustomerUniqueIdentifier} and pan {request.PAN}" });

            var response = await _cardSrvc.BlockCard(request);
            return response;

        }
        [HttpPost]
        [Route("ValidatePin")]
        public async Task<ValidatePinResponse> ValidatePin(ValidatePin request)
        {
         

            if (!ModelState.IsValid)
                return new ValidatePinResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside BlockCard for customer with uniqueid {request.PAN} and pan {request.PAN}" });

            var response = await _cardSrvc.ValidatePin(request);
            return response;

        }
        [HttpPost]
        [Route("LinkCard")]
        public async Task<CardLinkageResponse> LinkCard(CardLinkageRequest request)
        {
          

            if (!ModelState.IsValid)
                return new CardLinkageResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside LinkCard for customer with name {request.FullName} and pan {request.PAN}" });

            var response = await _cardSrvc.CardLinkage(request);
            return response;

        }
        [HttpPost]
        [Route("SetPin")]
        public async Task<SetPinResponse> SetPin(SetPin request)
        {
         

            if (!ModelState.IsValid)
                return new SetPinResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside BlockCard for customer with uniqueid {request.PAN} and pan {request.PAN}" });

            var response = await _cardSrvc.SetPin(request);
            return response;

        }
        [HttpPost]
        [Route("UnBlockCard")]
        public async Task<UnBlockCardOut> UnBlockCard(UnBlockCardIn request)
        {
           

            if (!ModelState.IsValid)
                return new UnBlockCardOut() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside UnBlockCard for customer with uniqueid {request.CustomerUniqueIdentifier} and pan {request.PAN}" });

            var response = await _cardSrvc.UnBlockCard(request);
            return response;

        }

        [HttpPost]
        [Route("EnableChannel")]
        public async Task<ChannelResponse> EnableChannel(ChannelRequest request)
        {
         

            if (!ModelState.IsValid)
                return new ChannelResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside EnableChannel for customer with uniqueid {request.CustomerUniqueIdentifier} and pan {request.PAN}" });

            var response = await _cardSrvc.EnableChannel(request);
            return response;

        }

        [HttpPost]
        [Route("EnableChannelForEnrolledCard")]
        public async Task<ChannelResponse> EnableChannelForEnrolledCard(ChannelEnrolledRequest request)
        {
          

            if (!ModelState.IsValid)
                return new ChannelResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside EnableChannelForEnrolledCard for pan {request.PAN}" });

            var response = await _cardSrvc.EnableChannelForEnrolledCard(request);
            return response;

        }


        [HttpPost]
        [Route("DisableChannel")]
        public async Task<ChannelResponse> DisableChannel(ChannelRequest request)
        {
          

            if (!ModelState.IsValid)
                return new ChannelResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside DisableChannel for customer with uniqueid {request.CustomerUniqueIdentifier} and pan {request.PAN}" });

            var response = await _cardSrvc.DisableChannel(request);
            return response;

        }

        [HttpPost]
        [Route("DisableChannelForEnrolledCard")]
        public async Task<ChannelResponse> DisableChannelForEnrolledCard(ChannelEnrolledRequest request)
        {
        

            if (!ModelState.IsValid)
                return new ChannelResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside DisableChannelForEnrolledCard for pan {request.PAN}" });

            var response = await _cardSrvc.DisableChannelForEnrolledCard(request);
            return response;

        }

        [HttpPost]
        [Route("GetEnabledChannel")]
        public async Task<GetChannelResponse> GetEnabledChannel(GetChannelRequest request)
        {
          

            if (!ModelState.IsValid)
                return new GetChannelResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside GetChannelResponse for pan {request.PAN}" });

            var response = await _cardSrvc.GetEnabledChannel(request);
            return response;

        }

        [HttpPost]
        [Route("GetCardDetail")]
        public async Task<CardDetailResponse> GetCardDetail(GetCardRequest request)
        {
          

            if (!ModelState.IsValid)
                return new CardDetailResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside GetCardDetail for pan {request.PAN}" });

            var response = await _cardSrvc.GetCardDetail(request);
            return response;

        }

        [HttpPost]
        [Route("GetActiveCard")]
        public async Task<GetCardResponse> GetActiveCard(GetCardsRequest request)
        {
          

            if (!ModelState.IsValid)
                return new GetCardResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside GetActiveCard for account {request.AccountId}" });

            var response = await _cardSrvc.GetActiveCard(request);
            return response;

        }

        [HttpPost]
        [Route("GetAllCard")]
        public async Task<GetCardResponse> GetAllCard(GetCardsRequest request) 
        {
         

            if (!ModelState.IsValid)
                return new GetCardResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside GetAllActiveCard for account {request.AccountId}" });

            var response = await _cardSrvc.GetAllActiveCardList(request);
            return response;

        }

        [HttpPost]
        [Route("GoMoneyMinistatement")]
        public async Task<GetStatementResponse> GoMoneyMinistatement(string accountNumber)
        {

            if (!ModelState.IsValid)
                return new GetStatementResponse() { status = "failure", ResponseCode = "-1", ResponseMessage = $"They were some errors in your input please try again" };

            await _logService.AddLog(new LogDto() { Type = "info", Description = $"Inside GoMoneyMinistatement for account {accountNumber}" });

            var response = await _cardSrvc.GetGoMoneyMinistatement(accountNumber);
            return response;

        }

        //[HttpPost]
        //[Route("RequestVirtualCard")]
        //public async Task<IApiResponse<VirtualCardResponse>> RequestVirtualCard(VirtualCardRequest request)
        //{
        //    //return await HandleApiOperationAsync(async () =>
        //    //{
        //        var response =await _cardSrvc.InstanceVirtualCardReqWallet(request);
        //        return new DefaultApiReponse<VirtualCardResponse>
        //        {
        //            Object = response
        //        };
        //    //});
        //}



        //[HttpPost]
        //[Route("BlockCard")]
        //public async Task<IApiResponse<BlockCardOut>> BlockCard(BlockCardIn request)
        //{
        //    return await HandleApiOperationAsync(async () =>
        //    {
        //        var response =await _cardSrvc.BlockCard(request);
        //        return new DefaultApiReponse<BlockCardOut>
        //        {
        //            Object = response
        //        };
        //    });
        //}

        //[HttpPost]
        //[Route("UnBlockCard")]
        //public async Task<IApiResponse<UnBlockCardOut>> UnBlockCard(UnBlockCardIn request)
        //{
        //    return await HandleApiOperationAsync(async () =>
        //    {
        //        var response = await _cardSrvc.UnBlockCard(request);
        //        return new DefaultApiReponse<UnBlockCardOut>
        //        {
        //            Object = response
        //        };
        //    });
        //}

    }
}
