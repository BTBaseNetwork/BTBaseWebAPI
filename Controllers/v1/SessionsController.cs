using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BTBaseWebAPI.DAL;
using BTBaseWebAPI.Models;
using BTBaseWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class SessionsController : Controller
    {
        private readonly BTBaseDbContext dbContext;
        private readonly SessionService sessionService;
        private readonly AccountService accountService;

        public SessionsController(BTBaseDbContext dbContext, SessionService sessionService, AccountService accountService)
        {
            this.dbContext = dbContext;
            this.sessionService = sessionService;
            this.accountService = accountService;
        }

        [HttpGet("DeviceAccount")]
        public object GetDeviceSession(bool active = false)
        {
            var session = active ?
            sessionService.ReactiveSession(dbContext, this.GetHeaderDeviceId()) :
            sessionService.GetSession(dbContext, this.GetHeaderDeviceId());

            if (session == null)
            {
                return new ApiResult
                {
                    code = this.SetResponseNotFound(),
                    msg = "Device Not Login"
                };
            }
            else
            {
                var account = accountService.GetProfile(dbContext, session.AccountId);
                return new ApiResult
                {
                    code = this.SetResponseOK(),
                    content = new { AccountId = account.AccountId, UserName = account.UserName, Nick = account.Nick }
                };
            }
        }

        [HttpPost]
        public object Login(string userstring, string password)
        {
            var account = accountService.GetValidateProfile(dbContext, userstring, password);

            if (account != null)
            {
                var session = sessionService.GetSession(dbContext, this.GetHeaderDeviceId(), account.AccountId, true);
                if (session == null)
                {
                    session = sessionService.NewSession(dbContext, new Models.BTDeviceSession
                    {
                        DeviceId = this.GetHeaderDeviceId(),
                        AccountId = account.AccountId,
                        DeviceName = this.GetHeaderDeviceName()
                    });
                }
                var logoutDevices = sessionService.InvalidSessionAccountLimited(dbContext, account.AccountId, 5);
                return new ApiResult
                {
                    code = this.SetResponseOK(),
                    content = GenerateSessionAccount(account, session, logoutDevices.Count() > 0 ? logoutDevices : null)
                };
            }
            else
            {
                return new ApiResult
                {
                    code = this.SetResponseNotFound(),
                    msg = "Validate Failed"
                };
            }
        }

        [HttpDelete]
        public object Logout()
        {
            var cnt = sessionService.InvalidAllSession(dbContext, this.GetHeaderAccountId(), this.GetHeaderDeviceId(), this.GetHeaderSession()).Count();
            return new ApiResult
            {
                code = cnt > 0 ? this.SetResponseOK() : this.SetResponseNotFound(),
                msg = "Devices Session Invalided:" + cnt,
                content = cnt
            };
        }

        private object GenerateSessionAccount(BTAccount account, BTDeviceSession session, IEnumerable<string> kickedDevices)
        {
            return new
            {
                AccountId = account.AccountId,
                UserName = account.UserName,
                Nick = account.Nick,
                Session = session.SessionKey,
                KickedDevices = kickedDevices
            };
        }
    }
}