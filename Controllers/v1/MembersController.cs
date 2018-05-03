using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    public class MembersController : Controller
    {
        [HttpPost("ExpiredDate/Order")]
        public void Recharge(string productId, string receiptData)
        {

        }
    }
}