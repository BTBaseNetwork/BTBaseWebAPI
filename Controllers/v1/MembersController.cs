using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTBaseWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class MembersController : Controller
    {
        private readonly DAL.BTBaseDbContext dbContext;
        private readonly Services.MemberService memberService;
        public MembersController(DAL.BTBaseDbContext dbContext, Services.MemberService memberService)
        {
            this.dbContext = dbContext;
            this.memberService = memberService;
        }

        [HttpGet("Profile")]
        public object GetProfile()
        {
            var profile = memberService.GetProfile(dbContext, this.GetHeaderAccountId());
            profile.ID = 0;
            return new ApiResult
            {
                code = 200,
                content = profile
            };
        }

        [HttpPost("ExpiredDate/Order")]
        public void Recharge(string productId, string channel, string receiptData, bool sandbox)
        {

        }
    }
}