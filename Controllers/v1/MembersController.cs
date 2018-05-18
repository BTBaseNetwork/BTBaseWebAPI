using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BTBaseServices;
using BTBaseServices.Models;
using BTBaseServices.DAL;
using BTBaseServices.Services;
using Microsoft.AspNetCore.Authorization;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class MembersController : Controller
    {

        [Authorize]
        [HttpGet("Profile")]
        public object GetProfile([FromServices]BTBaseDbContext dbContext, [FromServices]MemberService memberService)
        {
            var profile = memberService.GetProfile(dbContext, this.GetHeaderAccountId());
            return new ApiResult
            {
                code = this.SetResponseOK(),
                content = profile
            };
        }

        [Authorize]
        [HttpPost("ExpiredDate/Order")]
        public async Task<object> RechargeAsync([FromServices]BTBaseDbContext dbContext, [FromServices]MemberService memberService,
        string productId, string channel, string receiptData, bool sandbox)
        {
            switch (channel)
            {
                case BTServiceConst.CHANNEL_APP_STORE: return await memberService.VerifyReceiptAppStoreAsync(dbContext, this.GetHeaderAccountId(), productId, receiptData, sandbox);
                default: return new ApiResult { code = this.SetResponseForbidden(), msg = "Unsupported Channel" };
            }
        }
    }
}