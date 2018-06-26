using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers
{
    [Route("api")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "BTBase Web API Service";
        }

        // GET api/values/status
        [HttpGet("Status")]
        public object GetStatus([FromServices]BTBaseServices.DAL.BTBaseDbContext dbContext,
         [FromServices]BTBaseServices.Services.AccountService accountService,
         [FromServices]BTBaseServices.Services.SessionService sessionService)
        {
            var accountCount = accountService.CountAccount(dbContext);
            var now = DateTime.Now;

            var ts = TimeSpan.FromHours(now.Hour) + TimeSpan.FromMinutes(now.Minute) + TimeSpan.FromSeconds(now.Second);

            var onlineSession = sessionService.OnlineSessions(dbContext, ts);

            var todayNewAccount = accountService.CountNewAccount(dbContext, now - ts, now);
            return new
            {
                TotalAccounts = accountCount,
                TodayRegisters = todayNewAccount,
                TodayOnline = onlineSession
            };
        }
    }
}
