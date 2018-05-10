using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BTBaseWebAPI.Controllers;
using BTBaseServices.DAL;
using BTBaseServices.Services;
using BTBaseServices.Models;
using Microsoft.AspNetCore.Authorization;

namespace BTBaseWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {
        private readonly BTBaseDbContext dbContext;
        private readonly AccountService accountService;
        private readonly SessionService sessionService;

        public AccountsController(BTBaseDbContext dbContext, AccountService accountService, SessionService sessionService)
        {
            this.dbContext = dbContext;
            this.accountService = accountService;
            this.sessionService = sessionService;
        }

        [HttpPost]
        public object Regist(string username, string password)
        {
            if (!CommonRegexTestUtil.TestPattern(username, CommonRegexTestUtil.PATTERN_USERNAME))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "User Name Is Unsupport" };
            }

            if (!CommonRegexTestUtil.TestPattern(password, CommonRegexTestUtil.PATTERN_PASSWORD_HASH))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "Password Is Unsupport" };
            }

            if (!accountService.IsUsernameAvaiable(dbContext, username))
            {
                return new ApiResult { code = this.SetResponseForbidden(), msg = "User Name Is Registed" };
            }

            var newAccount = new BTAccount
            {
                UserName = username,
                Nick = username,
                Password = password
            };

            newAccount = accountService.CreateNewAccount(dbContext, newAccount);
            return new ApiResult
            {
                code = this.SetResponseOK(),
                msg = "Success",
                content = new
                {
                    AccountId = newAccount.AccountId,
                    UserName = newAccount.UserName
                }
            };

        }

        [Authorize]
        [HttpGet("Profile")]
        public object GetAccountProfile()
        {
            var ac = accountService.GetProfile(dbContext, this.GetHeaderAccountId());
            return new ApiResult
            {
                code = this.SetResponseOK(),
                msg = ac != null ? "Success" : "No Account",
                content = ac == null ? null : new
                {
                    AccountId = ac.AccountId,
                    UserName = ac.UserName,
                    AccountTypes = ac.AccountTypes,
                    Nick = ac.Nick,
                    Email = ac.Email,
                    Mobile = ac.Mobile,
                    SignDateTs = ac.SignDateTs
                }
            };
        }

        [HttpGet("Username/{username}")]
        public object CheckUsernameAvaiable(string username)
        {
            return new ApiResult
            {
                code = this.SetResponseOK(),
                content = accountService.IsUsernameAvaiable(dbContext, username)
            };
        }

        [Authorize]
        [HttpPut("Nick")]
        public object UpdateNickName([FromBody]string newNick)
        {
            return new ApiResult
            {
                code = this.SetResponseOK(),
                content = accountService.UpdateNick(dbContext, this.GetHeaderAccountId(), newNick)
            };
        }

        public class UpdatePasswordFrom
        {
            public string originPassword;
            public string newPassword;
        }

        [Authorize]
        [HttpPut("Password")]
        public object UpdatePassword([FromBody]UpdatePasswordFrom form)
        {
            return new ApiResult
            {
                code = this.SetResponseOK(),
                content = accountService.UpdatePassword(dbContext, this.GetHeaderAccountId(), form.originPassword, form.newPassword)
            };
        }
    }
}