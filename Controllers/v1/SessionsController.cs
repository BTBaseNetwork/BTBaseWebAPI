using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTBaseWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class SessionsController : Controller
    {
        private readonly DAL.BTBaseDbContext dbContext;
        private readonly Services.SessionService sessionService;
        public SessionsController(DAL.BTBaseDbContext dbContext, Services.SessionService sessionService)
        {
            this.dbContext = dbContext;
            this.sessionService = sessionService;
        }

        [HttpPost]
        public object Login(string userstring, string password)
        {
            var accountList = from a in dbContext.BTAccount where a.Password == password && (a.AccountId == userstring || a.UserName == userstring || a.Email == userstring || a.Mobile == userstring) select a;
            if (accountList.Any())
            {
                var account = accountList.First();
                var session = sessionService.NewSession(dbContext, new Models.BTDeviceSession
                {
                    DeviceId = this.GetHeaderDeviceId(),
                    AccountId = account.AccountId,
                    DeviceName = this.GetHeaderDeviceName()
                });
                var logoutDevices = sessionService.InvalidSessionAccountLimited(dbContext, account.AccountId, 5);
                return new ApiResult
                {
                    code = 200,
                    content = new
                    {
                        AccountId = session.AccountId,
                        Session = session.SessionKey,
                        KickedDevices = logoutDevices.Count() > 0 ? logoutDevices : null
                    }
                };
            }
            else
            {
                return new ApiResult
                {
                    code = 400,
                    msg = "Validate Failed"
                };
            }
        }

        [HttpDelete]
        public void Logout()
        {
            sessionService.InvalidAllSession(dbContext, this.GetHeaderAccountId(), this.GetHeaderDeviceId(), this.GetHeaderSession());
        }

        [HttpPost("ReactiveSession")]
        public object ReactiveSession()
        {
            var reatived = sessionService.ReactiveSession(dbContext, this.GetHeaderAccountId(), this.GetHeaderDeviceId(), this.GetHeaderSession());
            return new ApiResult
            {
                code = 200,
                msg = reatived ? "Session Reactived" : "No Session",
                content = reatived
            };
        }

        [HttpGet("Device")]
        public object GetDeviceSession()
        {
            var session = sessionService.GetSession(dbContext, this.GetHeaderDeviceId());
            return new ApiResult
            {
                code = 200,
                msg = session == null ? "No Session" : "Success",
                content = new
                {
                    AccountId = session.AccountId,
                    Session = session.SessionKey,
                }
            };
        }
    }
}