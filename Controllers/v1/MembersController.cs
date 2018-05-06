using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class MembersController : Controller
    {
        [HttpGet("Profile")]
        public void GetProfile()
        {

        }

        [HttpPost("ExpiredDate/Order")]
        public void Recharge(string productId, string receiptData, bool sandbox)
        {

        }
    }
}